# MultiFamilyPortal

The MultiFamilyPortal is configured as a Multi-Tenant SaaS application. This means that there is a single deployed instance of the application, with a unique database connection for each tenant. To facilitate this, the MultiFamilyPortal relies on two Database Connections:

- The Default Connection: This is a templated connection string in which the Initial Catalog or Database name must be provided as `{0}`. The name of the database configured for the tenant will be substituted for each tenant.
- The Tenant Connection: This is database that controls allowed tenants.

Both connection strings must be supplied for both the MultiFamilyPortal & the MultiFamlyPortal SaaS Admin Portal to function. By default while running this locally in debug mode, the MultiFamilyPortal will ensure that a Tenant exists for the host `localhost`. This will default to a database named `multifamilyportal`. You can customize this using the Admin portal.

When setting up a new environment it is important that you run the MultiFamilyPortal SaaS Admin portal first. You should ensure that the tenant exists for `localhost` while developing locally. You should also ensure that a database exists for the tenant which you will specify when adding the tenant. After setting up the tenant, you will need to open the Settings for the Tenant. You will need to add the settings for:

- Google Captcha (Site Key & Secret)
- Google Authentication (Client ID & Secret)
- Microsoft Account Authentication (Client ID & Secret)

## Configuration

Note both connection strings are expected for both portals, however ONLY the MultiFamilyPortal requires the PostmarkApiKey.

```json
{
  "PostmarkApiKey": "{Postmark Key}",
  "ConnectionString": {
    "DefaultConnection": "{Default Connection String}",
    "TenantConnection": "{Tenant Connection String}"
  }
}
```

### OAuth Configuration

The MultiFamilyPortal allows for the use of Google & Microsoft Accounts to authenticate users. The configuration for these providers is stored in each tenant's database. This ensures that in the event a tenant is compromised, the no database contains the client id and secret for all of the tenants. This can ONLY be configured however from the SaaS Tenant Admin Portal.

### Testing Multi Tenant Functionality

In order to locally test the Multi Tenant Functionality, you must provide an updated `hosts` configuration on your local machine. For instance let's say that you want to test the following two demo tenants:

- demoportal
- acmeequities

To do this you would provide an updated `hosts` configuration as follows:

```
127.0.0.1   demoportal
127.0.0.1   acmeequities
```

#### On Mac

You can edit the `hosts` file using the following command:

```bash
sudo nano /private/etc/hosts
```

#### On PC

Open the file `C:\Windows\System32\drivers\etc\hosts` and edit the file. You may do this using Notepad or Visual Studio Code. Note that if you are using Notepad you will need to run Notepad as the Administrator. If you are using Visual Studio Code, when saving or "Auto Saving" you may be prompted to "Retry as Administrator". It will require elevated privileges to save the file.

## Limitations

Currently the SaaS functionality is limited to only preparing the database for tenants at application startup and cannot prepare tenant databases from the SaaS Admin Portal or on Client Requests. This is intentional as the preparation process is expensive and should not be performed in the request pipeline. Additional work will likely need to be done to migrate this functionality to the SaaS Admin Portal.

## Database Migrations

Due to the complexity of the Multi-Tenant structure, Database Migrations are not supported directly with the `dotnet ef` cli tool. In order to perform a database migration, the override in the MFPContext for OnConfiguring must be temporarily removed. By removing this the DesignTimeFactory can specify a mock connection string and the tool can perform the migration.

```cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    this.ConfigureForMultiTenant(optionsBuilder, _databaseSettings, _tenantProvider.GetTenant());
}
```
