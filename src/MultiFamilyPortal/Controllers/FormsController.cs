using System.Net.Mail;
using AvantiPoint.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("/api/[controller]")]
    public class FormsController : ControllerBase
    {
        private IEmailService _emailService { get; }
        private IEmailValidationService _emailValidator { get; }
        private ITemplateProvider _templateProvider { get; }
        private IMFPContext _dbContext { get; }
        private IIpLookupService _ipLookup { get; }
        private ISiteInfo _siteInfo { get; }

        public FormsController(IMFPContext context,
            IEmailService emailService,
            IEmailValidationService emailValidationService,
            IIpLookupService ipLookupService,
            ISiteInfo siteInfo,
            ITemplateProvider templateProvider)
        {
            _dbContext = context;
            _emailService = emailService;
            _emailValidator = emailValidationService;
            _ipLookup = ipLookupService;
            _siteInfo = siteInfo;
            _templateProvider = templateProvider;
        }


        [HttpPost("contact-us")]
        public async Task<IActionResult> ContactUs([FromBody] ContactFormRequest form)
        {
            var validatorResponse = _emailValidator.Validate(form.Email);

            if (!validatorResponse.IsValid)
            {
                return BadRequest(new FormResult
                {
                    Errors = new Dictionary<string, List<string>>
                    {
                        { nameof(ContactFormRequest.Email), new List<string> { validatorResponse.Message } }
                    },
                    Message = validatorResponse.Message,
                    State = ResultState.Error
                });
            }

            var url = $"{Request.Scheme}://{Request.Host}";
            var notification = new ContactNotificationTemplate
            {
                Comments = form.Comments,
                Email = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName,
                Phone = form.Phone,
                Subject = "Contact Form Request"
            };
            var notificationMessage = await _templateProvider.GetTemplate(PortalTemplate.ContactNotification, notification);
            await _emailService.SendAsync(notificationMessage);

            var userNotification = new ContactFormEmailNotification
            {
                DisplayName = $"{form.FirstName} {form.LastName}".Trim(),
                Email = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName,
                Message = $"<p>Thank you for contacting us. One of our team members will be in touch shortly.</p>",
                SiteTitle = _siteInfo.Title,
                SiteUrl = url,
                Subject = $"Investor Request {_siteInfo.Title}",
                Year = DateTime.Now.Year
            };
            var userMessage = await _templateProvider.GetTemplate(PortalTemplate.ContactMessage, userNotification);
            var emailAddress = new MailAddress(form.Email, $"{form.FirstName} {form.LastName}".Trim());
            await _emailService.SendAsync(emailAddress, userMessage);

            return Ok(new FormResult
            {
                Message = "Success! Contact Request was successfully sent.",
                State = ResultState.Success
            });
        }

        [HttpPost("investor-inquiry")]
        public async Task<IActionResult> InvestorInquiry([FromBody]InvestorInquiryRequest form)
        {
            var validatorResponse = _emailValidator.Validate(form.Email);

            if (!validatorResponse.IsValid || form.LookingToInvest is null)
            {
                return BadRequest(new FormResult
                {
                    Errors = new Dictionary<string, List<string>>
                    {
                        { nameof(ContactFormRequest.Email), new List<string> { validatorResponse.Message } }
                    },
                    Message = validatorResponse.Message,
                    State = ResultState.Error
                });
            }

            await _dbContext.InvestorProspects.AddAsync(new InvestorProspect
            {
                Email = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName,
                LookingToInvest = form.LookingToInvest.Value,
                Phone = form.Phone,
                //Comments = form.Comments,
                Timezone = form.Timezone,
                Comments = form.Comments,
            });

            await _dbContext.SaveChangesAsync();

            var url = $"{Request.Scheme}://{Request.Host}";
            var notification = new InvestorInquiryNotificationTemplate
            {
                Comments = form.Comments,
                Email = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName,
                Phone = form.Phone,
                LookingToInvest = form.LookingToInvest.Value.ToString("C"),
                Timezone = form.Timezone,
                Subject = $"Investor Inquiry - {form.FirstName} {form.LastName}"
            };
            var notificationMessage = await _templateProvider.GetTemplate(PortalTemplate.InvestorNotification, notification);
            await _emailService.SendAsync(notificationMessage);

            var investorInquiryNotification = new ContactFormEmailNotification
            {
                DisplayName = $"{form.FirstName} {form.LastName}".Trim(),
                Email = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName,
                Message = $"<p>Thank you for contacting us. One of our team members will be in touch shortly.</p>",
                SiteTitle = _siteInfo.Title,
                SiteUrl = url,
                Subject = $"Investor Request {_siteInfo.Title}",
                Year = DateTime.Now.Year
            };
            var investorMessage = await _templateProvider.GetTemplate(PortalTemplate.ContactMessage, investorInquiryNotification);
            var emailAddress = new MailAddress(form.Email, $"{form.FirstName} {form.LastName}".Trim());
            await _emailService.SendAsync(emailAddress, investorMessage);

            return Ok(new FormResult
            {
                Message = "Success! Investor Inquiry was successfully sent. A member of our team will be in touch shortly!",
                State = ResultState.Success
            });
        }

        [HttpPost("newsletter-subscriber")]
        public async Task<IActionResult> NewsletterSubscriber([FromBody]NewsletterSubscriberRequest form)
        {
            var validatorResponse = _emailValidator.Validate(form.Email);

            if (!validatorResponse.IsValid)
            {
                return BadRequest(new FormResult
                {
                    Errors = new Dictionary<string, List<string>>
                    {
                        { nameof(ContactFormRequest.Email), new List<string> { validatorResponse.Message } }
                    },
                    Message = validatorResponse.Message,
                    State = ResultState.Error
                });
            }

            var blogContext = _dbContext as IBlogContext;
            if (await blogContext.Subscribers.AnyAsync(x => x.Email == form.Email))
            {
                return Ok(new FormResult
                {
                    Message = "You have already subscribed.",
                    State = ResultState.Success
                });
            }

            var ipData = await _ipLookup.LookupAsync(HttpContext.Connection.RemoteIpAddress, Request.Host.Value);

            var subscriber = new Subscriber {
                City = ipData.City,
                Continent = ipData.Continent,
                Email = form.Email,
                IpAddress = HttpContext.Connection.RemoteIpAddress,
                Country = ipData.Country,
                Region = ipData.Region,
            };
            blogContext.Subscribers.Add(subscriber);
            await blogContext.SaveChangesAsync();

            var url = $"{Request.Scheme}://{Request.Host}";
            var confirmationUrl = $"{url}/subscriber/confirmation/{subscriber.ConfirmationCode}";

            var notification = new ContactFormEmailNotification
            {
                DisplayName = form.Email,
                Email = form.Email,
                FirstName = form.Email,
                LastName = form.Email,
                Message = $"<p>Thank you for signing up!<br />Before we start sending you messages, please confirm that this email address belongs to you and that you would like to recieve messages from us. Don't worry, if you didn't sign up, you won't get anything from us unless your confirm you email address.</p><div class=\"text-center\"><p><a href=\"{confirmationUrl}\">Confirm Email</a><br />Link not working? Copy and paste this url into your browser: {confirmationUrl}</p>",
                SiteTitle = _siteInfo.Title,
                SiteUrl = url,
                Subject = $"Successfully subscribed to updates on {_siteInfo.Title}",
                Year = DateTime.Now.Year
            };
            var message = await _templateProvider.GetTemplate(PortalTemplate.ContactMessage, notification);
            await _emailService.SendAsync(form.Email, message);

            return Ok(new FormResult
            {
                Message = "Success! You have succesfully subscribed to our updates.",
                State = ResultState.Success
            });
        }
    }
}
