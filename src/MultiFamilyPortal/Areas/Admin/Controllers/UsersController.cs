using AvantiPoint.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = PortalRoles.PortalAdministrator)]
    [ApiController]
    [Route("/api/[area]/[controller]")]
    public class UsersController : ControllerBase
    {
        private IMFPContext _dbContext { get; }
        private UserManager<SiteUser> _userManager { get; }

        public UsersController(IMFPContext dbContext, UserManager<SiteUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var roles = await _dbContext.Roles.ToDictionaryAsync(x => x.Id, x => x.Name);
            var userRoles = await _dbContext.UserRoles.AsNoTracking().ToArrayAsync();

            var users = await _dbContext.Users
                .Select(x => new UserAccountResponse
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Id = x.Id,
                    Phone = x.PhoneNumber,
                    LocalAccount = string.IsNullOrEmpty(x.PasswordHash) == false
                })
                .ToArrayAsync();

            foreach(var user in users)
            {
                user.Roles = userRoles.Where(x => x.UserId == user.Id).Select(x => roles[x.RoleId]).ToArray();
            }

            return Ok(users.OrderBy(x => x.FirstName));
        }

        [HttpGet("subscribers")]
        public async Task<IActionResult> GetSubscribers()
        {
            var subscribers = await ((IBlogContext)_dbContext).Subscribers.ToArrayAsync();
            return Ok(subscribers);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserRequest request, [FromServices] IEmailService emailSender, [FromServices] ITemplateProvider templateProvider)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email))
            {
                return BadRequest();
            }
            else if (!ModelState.IsValid || !(request.Roles?.Any() ?? false))
            {
                return NoContent();
            }

            var user = new SiteUser(request.Email)
            {
                Email = request.Email.Trim().ToLowerInvariant(),
                EmailConfirmed = true,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = request.Phone,
                PhoneNumberConfirmed = !string.IsNullOrEmpty(request.Phone) && request.Phone.Length == 10,
            };

            var info = "Use your Microsoft or Google Account to login";
            var tip = "";
            var actionPoint = "<b>If you are not aware of this action, ignore this message.</b>";
            IdentityResult result = null;
            if (request.UseLocalAccount)
            {
                var password = "";
                try
                {
                    using var _client = new HttpClient();
                    var response = await _client.GetAsync("https://www.passwordrandom.com/query?command=password");
                    password = await response.Content.ReadAsStringAsync();
                    result = await _userManager.CreateAsync(user, password);
                }
                catch (Exception ex)
                {
                    return StatusCode(500,ex);
                }

                info = $"Your password is <b> {password}</b>";
                tip = "You can change your password in your profile.";
            }
            else
            {
                result = await _userManager.CreateAsync(user);
            }

            if (!result.Succeeded)
                return BadRequest();

            await _userManager.AddToRolesAsync(user, request.Roles);
            if (request.Roles.Contains(PortalRoles.PortalAdministrator) || request.Roles.Contains(PortalRoles.Underwriter))
            {
                var goals = new UnderwriterGoal
                {
                    UnderwriterId = user.Id,
                };
                await _dbContext.UnderwriterGoals.AddAsync(goals);
                await _dbContext.SaveChangesAsync();
            }

            // TODO: Email User confirmation 
            if (result.Succeeded)
            {
                var siteTitle = await _dbContext.GetSettingAsync<string>(PortalSetting.SiteTitle);
                var notification = new ContactFormEmailNotification
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Message = $"<p>A new account has been created at https://{HttpContext.Request.Host} with your email address. {info} .<br/>{tip}<br/> {actionPoint}</p>",
                    SiteTitle = siteTitle,
                    SiteUrl = $"https://{HttpContext.Request.Host}",
                    Subject = $"{siteTitle} - New Account Created",
                    Year = DateTime.Now.Year,
                };
                var template = await templateProvider.GetTemplate(PortalTemplate.ContactMessage, notification);
                await emailSender.SendAsync(user.Email, template);
            }
            
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(string id, [FromBody] CreateUserRequest request)
        {
            // only deals with roles
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return BadRequest();

            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles.Select(x => x).ToArray());
                await _dbContext.SaveChangesAsync();
                await _userManager.AddToRolesAsync(user, request.Roles);
                var goals = await _dbContext.UnderwriterGoals.FirstOrDefaultAsync(x => x.UnderwriterId == user.Id);

                if ((request.Roles.Contains(PortalRoles.PortalAdministrator) || request.Roles.Contains(PortalRoles.Underwriter)) && (goals == null))
                {
                    goals = new UnderwriterGoal
                    {
                        UnderwriterId = user.Id,
                    };
                    await _dbContext.UnderwriterGoals.AddAsync(goals);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    _dbContext.UnderwriterGoals.Remove(goals);
                    await _dbContext.SaveChangesAsync();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var exists = await _dbContext.Users.AnyAsync(x => x.Id == id);

            if (!exists)
                return NotFound();

            try
            {
                var user = await _dbContext.Users.FirstAsync(x => x.Id == id);
                await _userManager.DeleteAsync(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

            return NoContent();
        }
    }
}
