using System.Net.Mail;
using AvantiPoint.EmailService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.FirstRun.Models;
using MultiFamilyPortal.Services;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.FirstRun.Pages
{
    public partial class Index
    {
        private readonly FirstRunSetup _model = new();
        private bool _isBusy;

        [Inject]
        private IMFPContext _dbContext { get; set; }

        [Inject]
        private UserManager<SiteUser> _userManager { get; set; }

        [Inject]
        private ITemplateProvider _templateProvider { get; set; }

        [Inject]
        private IEmailService _emailService { get; set; }

        [Inject]
        private ISiteConfigurationValidator _siteValidator { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private PortalNotification notification { get; set; }

        private bool SiteInfoIsValid()
        {
            return !string.IsNullOrEmpty(_model.SiteTitle) && IsEmail(_model.SenderEmail) && !string.IsNullOrEmpty(_model.SenderEmailName);
        }

        private bool BusinessEntityIsValid()
        {
            return SiteInfoIsValid() && new[]
            {
                _model.LegalName,
                _model.PublicEmail,
                _model.City,
                _model.State,
                _model.PostalCode,
            }.All(x => !string.IsNullOrEmpty(x)) && IsEmail(_model.PublicEmail);
        }

        private bool AdminAccountIsValid()
        {
            var password = !_model.UsePassword || !string.IsNullOrEmpty(_model.Password) && _model.Password.Length > 5 && _model.Password == _model.ConfirmPassword;
            return BusinessEntityIsValid() && IsEmail(_model.AdminUser) && password && !string.IsNullOrEmpty(_model.FirstName) && !string.IsNullOrEmpty(_model.LastName) && !string.IsNullOrEmpty(_model.AdminPhone);
        }

        private static bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                _ = new MailAddress(email);
                var parts = email.Split('@');
                if (parts.Length == 2 && parts[1].Split('.').Length > 1)
                    return true;
            }
            catch
            {
            }

            return false;
        }

        private async Task OnFinish()
        {
            try
            {
                _isBusy = true;

                if (!AdminAccountIsValid())
                    return;

                var settings = new[]
                {
                    new Setting
                    {
                        Key = PortalSetting.SiteTitle,
                        Value = _model.SiteTitle
                    },
                    new Setting
                    {
                        Key = PortalSetting.NotificationEmailFrom,
                        Value = _model.SenderEmailName
                    },
                    new Setting
                    {
                        Key = PortalSetting.NotificationEmail,
                        Value = _model.SenderEmail
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactStreetAddress,
                        Value = _model.Address
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactCity,
                        Value = _model.City
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactState,
                        Value = _model.State
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactZip,
                        Value = _model.PostalCode
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactStreetAddress,
                        Value = _model.Address
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactEmail,
                        Value = _model.PublicEmail
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactPhone,
                        Value = _model.Phone
                    }
                }.Where(x => !string.IsNullOrEmpty(x.Value));

                if (settings.Any())
                {
                    foreach (var setting in settings)
                    {
                        var x = await _dbContext.Settings.FirstOrDefaultAsync(x => x.Key == setting.Key);
                        x.Value = setting.Value;
                        _dbContext.Settings.Update(x);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                var user = new SiteUser(_model.AdminUser)
                {
                    FirstName = _model.FirstName,
                    LastName = _model.LastName,
                    Email = _model.AdminUser,
                    PhoneNumber = _model.AdminPhone,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Title = "Site Owner",
                    Bio = "<p>Your account is all set up. Be sure to update your Bio in the Admin Portal.</p>"
                };

                IdentityResult result = null;
                if (_model.UsePassword)
                {
                    result = await _userManager.CreateAsync(user, _model.Password);
                }
                else
                {
                    result = await _userManager.CreateAsync(user);
                }

                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, new[] { PortalRoles.Underwriter, PortalRoles.BlogAuthor, PortalRoles.PortalAdministrator });
                }
                else
                {
                    notification.ShowError($"Unable to create the admin account: {result.Errors.FirstOrDefault()?.Description}");
                    return;
                }

                var goals = new UnderwriterGoal
                {
                    UnderwriterId = user.Id
                };
                _dbContext.UnderwriterGoals.Add(goals);

                var highlighted = new HighlightedUser
                {
                    Order = 1,
                    UserId = user.Id
                };
                _dbContext.HighlightedUsers.Add(highlighted);
                await _dbContext.SaveChangesAsync();

                var model = new Dtos.ContactFormEmailNotification
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Message = "<p>Congratulations your website has been configured and is now ready to use.</p>",
                    Subject = "Portal Configured",
                    Year = DateTime.Now.Year
                };
                var template = await _templateProvider.GetTemplate(PortalTemplate.ContactMessage, model);
                var emailAddress = new MailAddress(user.Email, user.DisplayName);
                await _emailService.SendAsync(emailAddress, template);

                _siteValidator.SetFirstRunTheme(null);
                _navigationManager.NavigateTo("/", true, true);
            }
            catch (Exception ex)
            {
                notification.ShowError(ex.Message);
            }
            finally
            {
                _isBusy = false;
            }
        }
    }
}
