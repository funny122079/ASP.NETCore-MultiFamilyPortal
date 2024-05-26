using System.Net.Mail;
using System.Security.Claims;
using AvantiPoint.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = PortalPolicy.AdminPortalViewer)]
    [ApiController]
    [Route("/api/[area]/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private IMFPContext _dbContext { get; }

        public UserProfileController(IMFPContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var roles = await _dbContext.Roles.ToArrayAsync();
                var userRoles = await _dbContext.UserRoles.ToArrayAsync();

                var email = User.FindFirstValue(ClaimTypes.Email);
                var user = await _dbContext.Users
                    .Include(x => x.Goals)
                    .Include(x => x.SocialLinks)
                    .ThenInclude(x => x.SocialProvider)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Email == email);

                var response = new SerializableUser
                {
                    Title = user?.Title,
                    Bio = user?.Bio,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    LocalAccount = !string.IsNullOrEmpty(user.PasswordHash),
                    PhoneNumber = user.PhoneNumber,
                    SocialLinks = user.SocialLinks
                };


                if (userRoles.Any(ur => ur.UserId == user.Id && roles.Any(r =>
                        r.Id == ur.RoleId && (r.Name == PortalRoles.PortalAdministrator ||
                                              r.Name == PortalRoles.Underwriter))))
                {
                    response.Goals = user.Goals;
                    if (response.Goals is null)
                    {
                        var goals = new UnderwriterGoal {UnderwriterId = user.Id};
                        await _dbContext.AddAsync(goals);
                        await _dbContext.SaveChangesAsync();
                        response.Goals = goals;
                    }
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("social-providers")]
        public async Task<IActionResult> GetSocialProviders()
        {
            return Ok(await _dbContext.SocialProviders.AsNoTracking().ToArrayAsync());
        }

        [HttpPost("update/links")]
        public async Task<IActionResult> UpdateSocialLinks([FromBody] IEnumerable<SocialLink> socialLinks)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
                var existingLinks = await _dbContext.SocialLinks.Where(x => x.UserId == user.Id).ToArrayAsync();

                if (existingLinks?.Any() ?? false)
                {
                    _dbContext.SocialLinks.RemoveRange(existingLinks);
                    await _dbContext.SaveChangesAsync();
                }

                var updatedLinks = socialLinks.Select(x =>
                {
                    x.UserId = user.Id;
                    return x;
                }).Where(x => !string.IsNullOrEmpty(x.Value));
                if (updatedLinks?.Any() ?? false)
                {
                    await _dbContext.SocialLinks.AddRangeAsync(updatedLinks);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

            return NoContent();
        }

        [Authorize(Roles = PortalRoles.Underwriter)]
        [HttpPost("update/goals")]
        public async Task<IActionResult> UpdateGoals([FromBody] UnderwriterGoal goals)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (goals.UnderwriterId != user.Id)
                return BadRequest();

            if (goals.PropertiesUnderwritten < 0)
                goals.PropertiesUnderwritten = 0;

            if (goals.InvestorsContacted < 0)
                goals.InvestorsContacted = 0;

            if (goals.LOISubmitted < 0)
                goals.LOISubmitted = 0;

            if (goals.BrokersContacted < 0)
                goals.BrokersContacted = 0;

            if (await _dbContext.UnderwriterGoals.AnyAsync(x => x.Id == goals.Id))
            {
                _dbContext.UnderwriterGoals.Update(goals);
            }
            else
            {
                await _dbContext.UnderwriterGoals.AddAsync(goals);
            }

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("update/profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] SerializableUser model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Title = model.Title;
            user.PhoneNumber = model.PhoneNumber;
            user.Bio = model.Bio;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("update/password")]
        public async Task<IActionResult> UpdatePassword(
            [FromBody] ChangePasswordRequest request,
            [FromServices] UserManager<SiteUser> userManager,
            [FromServices] ITemplateProvider templateProvider,
            [FromServices] IEmailService emailService)
        {
            if (string.IsNullOrEmpty(request.Password) || request.Password != request.ConfirmPassword)
                return BadRequest();

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return BadRequest();

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.Password);

            var siteTitle = await _dbContext.GetSettingAsync<string>(PortalSetting.SiteTitle);
            var notification = new ContactFormEmailNotification {
                DisplayName = user.DisplayName,
                Email = email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Message = "<p>Your password has been changed. If you did not make this change please contact us so that we can reset your account.</p>",
                SiteTitle = siteTitle,
                SiteUrl = $"https://{HttpContext.Request.Host}",
                Subject = $"{siteTitle} - Password Reset",
                Year = DateTime.Now.Year,
            };
            var templateResult = await templateProvider.GetTemplate(PortalTemplate.ContactMessage, notification);

            var emailAddress = new MailAddress(user.Email, user.DisplayName);
            await emailService.SendAsync(emailAddress, templateResult);

            return Ok(result);
        }
    }
}
