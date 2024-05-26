using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.SaaS.Data;
using MultiFamilyPortal.SaaS.Models;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.SaaSAdmin.Pages
{
    public partial class Index
    {
        [Inject]
        private TenantContext _context { get; set; } = default!;

        [Inject]
        private IWebHostEnvironment _hostEnvironment { get; set; } = default!;

        private IEnumerable<Tenant> _tenants = default!;
        private readonly ObservableRangeCollection<Tenant> _filteredTenants = new ();

        private Tenant? _newTenant;
        private Tenant? _editTenant;
        private TelerikNotification? _notification;
        private bool _loading;
        private string _query = string.Empty;

        private readonly IEnumerable<string> _environments = new[]
        {
            "Development",
            "Staging",
            "Production"
        };

        protected override async Task OnInitializedAsync()
        {
            await UpdateTenants();
        }

        private async Task UpdateTenants()
        {
            _loading = true;
            _tenants = await _context.Tenants
                .AsNoTracking()
                .ToArrayAsync();
            FilterResults();

            _loading = false;
        }

        private void FilterResults()
        {
            if (!_tenants.Any())
                _filteredTenants.Clear();
            else if (string.IsNullOrEmpty(_query))
                _filteredTenants.ReplaceRange(_tenants);
            else
                _filteredTenants.ReplaceRange(_tenants.Where(x => x.Host.Contains(_query, StringComparison.InvariantCultureIgnoreCase)));
            StateHasChanged();
        }

        private void OnAddTenantClicked()
        {
            _newTenant = new Tenant
            {
                Environment = _hostEnvironment.EnvironmentName
            };
        }

        private void CloseNewTenant()
        {
            _newTenant = null;
        }

        private async Task SaveTenant()
        {
            if (_newTenant is null || _notification is null)
                return;

            if (string.IsNullOrEmpty(_newTenant.Host))
            {
                Warn("You must enter a host name");
                return;
            }
            else if(string.IsNullOrEmpty(_newTenant.DatabaseName))
            {
                Warn("You must enter a database name");
                return;
            }
            else if(string.IsNullOrEmpty(_newTenant.Environment))
            {
                Warn("You must enter an environment name");
                return;
            }
            else if(await _context.Tenants.AnyAsync(x => x.Host == _newTenant.Host))
            {
                Warn($"The host name {_newTenant.Host} already exists");
                return;
            }

            _newTenant.Host = _newTenant.Host.ToLower();
            _newTenant.Created = DateTimeOffset.Now;

            await _context.Tenants.AddAsync(_newTenant);
            await _context.SaveChangesAsync();

            _newTenant = null;
            await UpdateTenants();
        }

        private async Task OnUpdateTenant(GridCommandEventArgs args)
        {
            var updated = args.Item as Tenant;
            if (updated == null)
                return;

            var existing = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == updated.Id);

            if (existing is null)
                return;

            existing.DatabaseName = updated.DatabaseName;
            existing.Disabled = updated.Disabled;
            existing.Environment = updated.Environment;

            _context.Tenants.Update(existing);
            await _context.SaveChangesAsync();
            await UpdateTenants();
        }

        private async Task OnDeleteTenant(GridCommandEventArgs args)
        {
            var tenant = args.Item as Tenant;
            if (tenant == null)
                return;

            var existing = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == tenant.Id);

            if(existing is not null)
            {
                _context.Tenants.Remove(existing);
                await _context.SaveChangesAsync();
            }

            await UpdateTenants();
        }

        private void Warn(string message)
        {
            _notification?.Show(new NotificationModel
            {
                Text = message,
                ThemeColor = "warning"
            });
        }

        private void OnShowSettings(GridCommandEventArgs args)
        {
            _editTenant = args.Item as Tenant;
        }
    }
}
