using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = PortalPolicy.Underwriter)]
    [ApiController]
    [Route("/api/[area]/[controller]")]
    public class ContactsController : ControllerBase
    {
        private ICRMContext _dbContext { get; }

        public ContactsController(ICRMContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("userImage/{id?}")]
        public async Task<IActionResult> GetUserImage(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                if(user is not null)
                {
                    return Redirect(GravatarHelper.GetUri(user.Email, 90));
                }
            }

            return Redirect(GravatarHelper.GetUri("system@multifamilyportal.app", 90, DefaultGravatar.Wavatar));
        }


        [HttpGet("investors")]
        public async Task<IActionResult> Investors()
        {
            var investorRole = await _dbContext.Roles.Where(x => x.Name == PortalRoles.Investor).Select(x => x.Id).FirstAsync();
            var sponsorRole = await _dbContext.Roles.Where(x => x.Name == PortalRoles.Sponsor).Select(x => x.Id).FirstAsync();
            var mapping = new Dictionary<string, string>
            {
                { investorRole, PortalRoles.Investor },
                { sponsorRole, PortalRoles.Sponsor },
            };

            var userIds = await _dbContext.UserRoles.Where(x => x.RoleId == investorRole || x.RoleId == sponsorRole)
                .ToArrayAsync();
            var grouped = userIds.GroupBy(x => x.UserId);
            var users = new List<UserAccountResponse>();
            foreach (var user in grouped)
            {
                var userAccount = await _dbContext.Users
                    .Select(x => new UserAccountResponse
                    {
                        Email = x.Email,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Id = x.Id,
                        Phone = x.PhoneNumber,
                        LocalAccount = string.IsNullOrEmpty(x.PasswordHash) == false,
                    })
                    .FirstAsync(x => x.Id == user.Key);

                userAccount.Roles = user.Select(x => mapping[x.RoleId]).ToArray();
                users.Add(userAccount);
            }

            return Ok(users);
        }

        [HttpGet("investors/prospects")]
        public async Task<IActionResult> GetInvestorProspects()
        {
            var prospects = await _dbContext.InvestorProspects.ToArrayAsync();
            return Ok(prospects);
        }

        [HttpGet("crm-roles")]
        public async Task<IActionResult> GetCrmRoles()
        {
            var roles = await _dbContext.CrmContactRoles.ToArrayAsync();
            return Ok(roles);
        }

        [HttpPost("crm-role/create")]
        public async Task<IActionResult> CreateCrmRole([Bind("Name", "CoreTeam")]CRMContactRole role)
        {
            if (string.IsNullOrEmpty(role.Name))
                return BadRequest();
            else if (await _dbContext.CrmContactRoles.AnyAsync(x => x.Name == role.Name))
                return Conflict();

            await _dbContext.CrmContactRoles.AddAsync(role);
            await _dbContext.SaveChangesAsync();
            return StatusCode(201);
        }

        [HttpPut("crm-role/update/{id:guid}")]
        public async Task<IActionResult> UpdateCrmRole(Guid id, [Bind("Id", "Name", "CoreTeam")]CRMContactRole updated)
        {
            if (id != updated.Id || string.IsNullOrEmpty(updated.Name))
                return BadRequest();

            var role = await _dbContext.CrmContactRoles.FirstOrDefaultAsync(x => x.Id == id && x.SystemDefined == false);
            if (role == null)
                return NotFound();

            role.Name = updated.Name;
            role.CoreTeam = updated.CoreTeam;
            _dbContext.CrmContactRoles.Update(role);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("crm-role/delete/{id:guid}")]
        public async Task<IActionResult> DeleteCrmRole(Guid id)
        {
            if (id == default)
                return BadRequest();

            var role = await _dbContext.CrmContactRoles.FirstOrDefaultAsync(x => x.Id == id && x.SystemDefined == false);
            if (role is null)
                return Ok();

            _dbContext.CrmContactRoles.Remove(role);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("crm-contacts")]
        public async Task<IActionResult> GetCrmContacts()
        {
            var contacts = await _dbContext.CrmContacts
                .Include(x => x.Addresses)
                .Include(x => x.Emails)
                .Include(x => x.Phones)
                .Include(x => x.Roles)
                .Include(x => x.Markets)
                .ToArrayAsync();

            return Ok(contacts);
        }

        [HttpGet("crm-contact/{id:guid}")]
        public async Task<IActionResult> GetCrmContact(Guid id)
        {
            var contact = await _dbContext.CrmContacts
                .Include(x => x.Addresses)
                .Include(x => x.Reminders)
                .Include(x => x.Emails)
                .Include(x => x.Logs)
                .Include(x => x.Markets)
                .Include(x => x.NotableDates)
                .Include(x => x.Phones)
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == id);

            return Ok(contact);
        }

        [HttpPost("crm-contact/create")]
        public async Task<IActionResult> CreateCrmContact([Bind("Prefix", "FirstName", "MiddleName", "LastName", "Suffix", "Company", "Title", "DoB", "LicenseNumber", "Roles", "Addresses", "Emails", "Phones")]CRMContact newContact)
        {
            var contact = new CRMContact
            {
                Prefix = newContact.Prefix,
                FirstName = newContact.FirstName,
                MiddleName = newContact.MiddleName,
                LastName = newContact.LastName,
                Suffix = newContact.Suffix,
                Company = newContact.Company,
                Title = newContact.Title,
                LicenseNumber = newContact.LicenseNumber,
                DoB = newContact.DoB,
            };

            await _dbContext.CrmContacts.AddAsync(contact);
            await _dbContext.SaveChangesAsync();

            contact = await _dbContext.CrmContacts
                .Include(x => x.Addresses)
                .Include(x => x.Emails)
                .Include(x => x.Phones)
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == contact.Id);

            if(newContact?.Addresses?.Any() ?? false)
            {
                foreach(var address in newContact.Addresses)
                {
                    address.ContactId = contact.Id;
                    contact.Addresses.Add(address);
                }

                _dbContext.Update(contact);
                await _dbContext.SaveChangesAsync();
            }

            if (newContact?.Emails?.Any() ?? false)
            {
                var tempEmails = newContact.Emails.ToArray();
                foreach (var email in tempEmails)
                {
                    if (string.IsNullOrEmpty(email?.Email))
                        continue;

                    email.ContactId = contact.Id;
                    if (newContact.Emails.Count == 1)
                        email.Primary = true;
                    else if (!newContact.Emails.Any(x => x.Primary == true) && email.Email == newContact.Emails.First().Email)
                        email.Primary = true;
                    email.Email = email.Email.Trim().ToLowerInvariant();

                    contact.Emails.Add(email);
                }

                _dbContext.Update(contact);
                await _dbContext.SaveChangesAsync();
            }

            if (newContact?.Phones?.Any() ?? false)
            {
                var tempPhones = newContact.Phones.ToArray();
                foreach (var phone in tempPhones)
                {
                    if (string.IsNullOrEmpty(phone?.Number))
                        continue;

                    phone.ContactId = contact.Id;
                    if (newContact.Phones.Count == 1)
                        phone.Primary = true;
                    else if (!newContact.Phones.Any(x => x.Primary == true) && phone.Number == newContact.Phones.First().Number)
                        phone.Primary = true;

                    contact.Phones.Add(phone);
                }

                _dbContext.Update(contact);
                await _dbContext.SaveChangesAsync();
            }

            if (newContact?.Roles?.Any() ?? false)
            {
                foreach (var role in newContact.Roles)
                {
                    var dbRole = await _dbContext.CrmContactRoles.FirstOrDefaultAsync(x => x.Id == role.Id);
                    if (dbRole != null)
                        contact.Roles.Add(dbRole);
                }

                if (contact.Roles.Any())
                {
                    _dbContext.Update(contact);
                    await _dbContext.SaveChangesAsync();
                }
            }

            return StatusCode(201);
        }

        [HttpPut("crm-contact/update/{id:guid}")]
        public async Task<IActionResult> UpdateCrmContact(Guid id, CRMContact updatedContact)
        {
            if (id != updatedContact.Id)
                return BadRequest();

            var contact = await _dbContext.CrmContacts
                .Include(x => x.Addresses)
                .Include(x => x.Reminders)
                .Include(x => x.Emails)
                .Include(x => x.Phones)
                .Include(x => x.Logs)
                .Include(x => x.Markets)
                .Include(x => x.NotableDates)
                .Include(x => x.Reminders)
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (contact is null)
                return NotFound();

            contact.Prefix = updatedContact.Prefix?.Trim();
            contact.FirstName = updatedContact.FirstName?.Trim();
            contact.MiddleName = updatedContact.MiddleName?.Trim();
            contact.LastName = updatedContact.LastName?.Trim();
            contact.Suffix = updatedContact.Suffix?.Trim();
            contact.Company = updatedContact.Company?.Trim();
            contact.Title = updatedContact.Title?.Trim();
            contact.LicenseNumber = updatedContact.LicenseNumber?.Trim();
            contact.DoB = updatedContact.DoB;
            contact.MarketNotes = updatedContact.MarketNotes?.Trim();

            if(!contact.Roles.All(x => updatedContact.Roles.Any(r => r.Id == x.Id)))
            {
                contact.Roles.Clear();
                foreach(var role in updatedContact.Roles)
                {
                    if (role.Id == default)
                        continue;

                    var dbRole = await _dbContext.CrmContactRoles.FirstOrDefaultAsync(x => x.Id == role.Id);
                    if(dbRole is not null)
                    {
                        contact.Roles.Add(dbRole);
                    }
                }
            }

            if (updatedContact.Addresses?.Any() ?? false)
            {
                if (updatedContact.Addresses.Count(x => x.Primary == true) != 1)
                {
                    var address = updatedContact.Addresses.FirstOrDefault();
                    if (address is not null)
                        address.Primary = true;
                }

                var newAddresses = updatedContact.Addresses.Where(x => x.Id == default);
                var existingAddresses = updatedContact.Addresses.Where(x => x.Id != default);
                var deletedAddresses = contact.Addresses.Where(x => !updatedContact.Addresses.Any(e => e.Id == x.Id));

                if (deletedAddresses?.Any() ?? false)
                {
                    _dbContext.CrmContactAddresses.RemoveRange(deletedAddresses);
                }

                foreach (var updated in existingAddresses)
                {
                    var address = contact.Addresses.FirstOrDefault(x => x.Id == updated.Id);
                    if (address is null)
                        continue;

                    address.Address1 = updated.Address1?.Trim();
                    address.Address2 = updated.Address2?.Trim();
                    address.City = updated.City?.Trim();
                    address.State = updated.State?.Trim();
                    address.PostalCode = updated.PostalCode?.Trim();
                    address.Type = updated.Type;
                    address.Primary = updated.Primary;
                    _dbContext.CrmContactAddresses.Update(address);
                }

                foreach (var added in newAddresses)
                {
                    var newAddress = new CRMContactAddress
                    {
                        ContactId = contact.Id,
                        Address1 = added.Address1,
                        Address2 = added.Address2,
                        City = added.City,
                        State = added.State,
                        PostalCode = added.PostalCode,
                        Primary = added.Primary,
                        Type = added.Type
                    };
                    await _dbContext.CrmContactAddresses.AddAsync(newAddress);
                }
            }
            else
            {
                contact.Addresses = null;
            }

            if (updatedContact.Emails?.Any() ?? false)
            {
                if(updatedContact.Emails.Count(x => x.Primary == true) != 1)
                {
                    var email = updatedContact.Emails.FirstOrDefault();
                    if (email is not null)
                        email.Primary = true;
                }

                var newEmails = updatedContact.Emails.Where(x => x.Id == default);
                var existingEmails = updatedContact.Emails.Where(x => x.Id != default);
                var deletedEmails = contact.Emails.Where(x => !updatedContact.Emails.Any(e => e.Id == x.Id));

                if(deletedEmails?.Any() ?? false)
                {
                    _dbContext.CrmContactEmails.RemoveRange(deletedEmails);
                }

                foreach(var updated in existingEmails)
                {
                    var email = contact.Emails.FirstOrDefault(x => x.Id == updated.Id);
                    if (email is null)
                        continue;

                    email.Email = updated.Email.Trim().ToLowerInvariant();
                    email.Type = updated.Type;
                    email.Primary = updated.Primary;
                    _dbContext.CrmContactEmails.Update(email);
                }

                foreach(var added in newEmails)
                {
                    var sanitizedEmail = added.Email.Trim().ToLowerInvariant();
                    if (await _dbContext.CrmContactEmails.AnyAsync(x => x.ContactId == contact.Id && x.Email == sanitizedEmail))
                        continue;

                    var newEmail = new CRMContactEmail
                    {
                        ContactId = contact.Id,
                        Email = sanitizedEmail,
                        Primary = added.Primary,
                        Type = added.Type
                    };
                    await _dbContext.CrmContactEmails.AddAsync(newEmail);
                }
            }

            if (updatedContact.Phones?.Any() ?? false)
            {
                if (updatedContact.Phones.Count(x => x.Primary == true) != 1)
                {
                    var phone = updatedContact.Phones.FirstOrDefault();
                    if (phone is not null)
                        phone.Primary = true;
                }

                var newPhones = updatedContact.Phones.Where(x => x.Id == default);
                var existingPhones = updatedContact.Phones.Where(x => x.Id != default);
                var deletedPhones = contact.Phones.Where(x => !updatedContact.Phones.Any(e => e.Id == x.Id));

                if (deletedPhones?.Any() ?? false)
                {
                    _dbContext.CrmContactPhones.RemoveRange(deletedPhones);
                }

                foreach (var updated in existingPhones)
                {
                    var phone = contact.Phones.FirstOrDefault(x => x.Id == updated.Id);
                    if (phone is null)
                        continue;

                    phone.Number = updated.Number;
                    phone.Type = updated.Type;
                    phone.Primary = updated.Primary;
                    _dbContext.CrmContactPhones.Update(phone);
                }

                foreach (var added in newPhones)
                {
                    if (await _dbContext.CrmContactPhones.AnyAsync(x => x.ContactId == contact.Id && x.Number == added.Number))
                        continue;

                    var newPhone = new CRMContactPhone
                    {
                        ContactId = contact.Id,
                        Number = added.Number,
                        Primary = added.Primary,
                        Type = added.Type
                    };
                    await _dbContext.CrmContactPhones.AddAsync(newPhone);
                }
            }

            if (updatedContact.Markets?.Any() ?? false)
            {
                var markets = await _dbContext.CrmContactMarkets.ToArrayAsync();
                var newMarkets = updatedContact.Markets.Where(x => !markets.Any(m => m.Name == x.Name));
                if(newMarkets?.Any() ?? false)
                {
                    await _dbContext.CrmContactMarkets.AddRangeAsync(newMarkets);
                    await _dbContext.SaveChangesAsync();
                    markets = await _dbContext.CrmContactMarkets.ToArrayAsync();
                }

                if(contact.Markets.Count != updatedContact.Markets.Count ||
                   !contact.Markets.All(x => updatedContact.Markets.Any(m => m.Name == x.Name)))
                {
                    contact.Markets.Clear();
                    foreach(var market in markets.Where(x => updatedContact.Markets.Any(m => m.Name == x.Name)))
                    {
                        contact.Markets.Add(market);
                    }
                }
            }

            if (updatedContact.NotableDates?.Any() ?? false)
            {
                var newDates = updatedContact.NotableDates.Where(x => x.Id == default);
                var existingDates = updatedContact.NotableDates.Where(x => x.Id != default);
                var deletedDates = contact.NotableDates.Where(x => !updatedContact.NotableDates.Any(e => e.Id == x.Id));

                if (deletedDates?.Any() ?? false)
                {
                    _dbContext.CrmNotableDates.RemoveRange(deletedDates);
                }

                foreach (var updated in existingDates)
                {
                    var date = contact.NotableDates.FirstOrDefault(x => x.Id == updated.Id);
                    if (date is null)
                        continue;

                    date.Date = updated.Date;
                    date.Description = updated.Description.Trim();
                    date.DismissReminders = updated.DismissReminders;
                    date.Recurring = updated.Recurring;
                    date.Type = updated.Type;
                    _dbContext.CrmNotableDates.Update(date);
                }

                foreach (var added in newDates)
                {
                    var newDate = new CRMNotableDate
                    {
                        ContactId = contact.Id,
                        Date = added.Date,
                        Description = added.Description.Trim(),
                        DismissReminders = added.DismissReminders,
                        Recurring = added.Recurring,
                        Type = added.Type
                    };
                    await _dbContext.CrmNotableDates.AddAsync(newDate);
                }
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (updatedContact.Logs?.Any() ?? false)
            {
                var newLogs = updatedContact.Logs.Where(x => x.Id == default);
                var existingLogs = updatedContact.Logs.Where(x => x.Id != default);

                foreach (var updated in existingLogs)
                {
                    // TODO: Do we lock this after a certain amount of time?
                    var log = contact.Logs.FirstOrDefault(x => x.UserId == userId && x.Id == updated.Id);
                    if (log is null)
                        continue;

                    if (log.Notes != updated.Notes)
                    {
                        log.Notes = updated.Notes;
                        _dbContext.CrmContactLogs.Update(log);
                    }
                }

                foreach (var added in newLogs)
                {
                    var newLog = new CRMContactLog
                    {
                        ContactId = contact.Id,
                        UserId = userId,
                        Notes = added.Notes,
                    };
                    await _dbContext.CrmContactLogs.AddAsync(newLog);
                }
            }

            if (updatedContact.Reminders?.Any() ?? false)
            {
                var newReminders = updatedContact.Reminders.Where(x => x.Id == default);
                var existingReminders = updatedContact.Reminders.Where(x => x.Id != default);
                var deletedReminders = contact.Reminders.Where(x => !updatedContact.Reminders.Any(e => e.Id == x.Id));

                if (deletedReminders?.Any() ?? false)
                {
                    _dbContext.CrmContactReminders.RemoveRange(deletedReminders);
                }

                foreach (var updated in existingReminders)
                {
                    var reminder = contact.Reminders.FirstOrDefault(x => x.Id == updated.Id);
                    if (reminder is null)
                        continue;

                    reminder.Date = updated.Date;
                    reminder.Description = updated.Description.Trim();
                    reminder.Dismissed = updated.Dismissed;
                    reminder.SystemGenerated = updated.SystemGenerated;
                    reminder.ContactId = contact.Id;
                    _dbContext.CrmContactReminders.Update(reminder);
                }

                foreach (var added in newReminders)
                {
                    var newReminder = new CRMContactReminder
                    {
                        ContactId = contact.Id,
                        Date = added.Date,
                        Description = added.Description.Trim(),
                        Dismissed = added.Dismissed,
                        SystemGenerated = added.SystemGenerated
                    };

                    await _dbContext.CrmContactReminders.AddAsync(newReminder);
                }
            }
            else
            {
                contact.Reminders = null;
            }

            _dbContext.CrmContacts.Update(contact);
            await _dbContext.SaveChangesAsync();

            var toDelete = await _dbContext.CrmContactMarkets.Where(x => x.Contacts.Count == 0).ToArrayAsync();
            if (toDelete?.Any() ?? false)
            {
                _dbContext.CrmContactMarkets.RemoveRange(toDelete);
                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
