using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;
using vCardLib.Enums;
using vCardLib.Models;
using vCardLib.Serializers;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("/api/[controller]")]
    public class AboutController : ControllerBase
    {
        private IMFPContext _dbContext { get; }

        public AboutController(IMFPContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> GetHighlightedUsers()
        {
            var users = await _dbContext.HighlightedUsers
                .Include(x => x.User)
                    .ThenInclude(x => x.SocialLinks)
                        .ThenInclude(x => x.SocialProvider)
                .ToListAsync();

            var response = users.OrderBy(x => x.Order)
                .Select(x => new HighlightedUserResponse
                {
                    Bio = x.User.Bio,
                    DisplayName = x.User.DisplayName,
                    Email = x.User.Email,
                    Phone = x.User.PhoneNumber,
                    Links = x.User.SocialLinks.Select(s => new SocialLinkResponse
                    {
                        Icon = s.SocialProvider.Icon,
                        Name = s.SocialProvider.Name,
                        Link = s.Uri.ToString()
                    }),
                    Title = x.User.Title,
                });

            return Ok(response);
        }

        [HttpGet("profile/{firstName}/{lastName}")]
        public async Task<IActionResult> GetUserProfile(string firstName, string lastName)
        {
            var user = await _dbContext.Users
                .Include(x => x.SocialLinks)
                    .ThenInclude(x => x.SocialProvider)
                .FirstOrDefaultAsync(x => x.FirstName == firstName && x.LastName == lastName);

            if (user is null)
                return NotFound();

            var response = new HighlightedUserResponse
            {
                Id = user.Id,
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Links = user.SocialLinks
                    .Select(x => new SocialLinkResponse
                    {
                        Icon = x.SocialProvider.Icon,
                        Link = x.Uri.ToString(),
                        Name = x.SocialProvider.Name,
                    }),
                Phone = user.PhoneNumber
            };

            return Ok(response);
        }

        [HttpGet("profile/vcard/{userId}")]
        public async Task<IActionResult> DownloadVCard(string userId, [FromServices]IIpLookupService _ipLookup)
        {
            var profile = await _dbContext.Users
                .Include(x => x.SocialLinks)
                    .ThenInclude(x => x.SocialProvider)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (profile is null)
                return Redirect("/not-found");

            var ipData = await _ipLookup.LookupAsync(HttpContext.Connection.RemoteIpAddress, Request.Host.Value);

            var card = new vCard
            {
                Version = vCardVersion.V3,
                Organization = _dbContext.GetSetting<string>(PortalSetting.LegalBusinessName),
                Title = profile.Title,
                Kind = ContactType.Individual,
                Language = "en-US",
                EmailAddresses = new List<EmailAddress>
                {
                    new EmailAddress{ Value = profile.Email, Type = EmailAddressType.Work }
                },
                GivenName = profile.FirstName,
                FamilyName = profile.LastName,
                Note = @$"Contact added {DateTime.Now:D}
Added from {ipData.Ip}
On or near: {ipData.City}, {ipData.Region}",
                Pictures = new List<Photo>
                {
                    new()
                    {
                        Encoding = PhotoEncoding.JPEG,
                        Type = PhotoType.URL,
                        PhotoURL = GravatarHelper.GetUri(profile.Email, 180, DefaultGravatar.MysteryPerson)
                    }
                },
                CustomFields = new List<KeyValuePair<string, string>>()
            };

            foreach (var link in profile.SocialLinks)
            {
                var key = $"socialProfile;type={link.SocialProvider.Name.ToLower()}";
                var uri = string.Format(link.SocialProvider.UriTemplate, link.Value);
                card.CustomFields.Add(new KeyValuePair<string, string>(key, uri));
            }

            var fileName = $"{profile.FirstName}{profile.LastName}.vcf".Replace(" ", "").Trim();
            var data = Encoding.Default.GetBytes(Serializer.Serialize(card));
            return File(data, "text/x-vcard", fileName);
        }

        //[HttpGet("qrcode/{firstName}-{lastName}/")]
        //public IActionResult UserQRCode(string firstName, string lastName)
        //{
        //    var contents = $"https://{Request.Host}/profile/{firstName}-{lastName}";
        //    var generator = new QRCodeGenerator();
        //    var qr = generator.CreateQrCode(contents, ECCLevel.H, eciMode: QRCodeGenerator.EciMode.Utf8);
        //    qr.


        //    var output = new MemoryStream();
        //    // generate QRCode
        //    var qrCode = new QrCode(, new Vector2Slim(512, 512), SKEncodedImageFormat.Png);

        //    // output to file
        //    qrCode.GenerateImage(output);
        //    return new FileStreamResult(output, "image/png");
        //}
    }
}
