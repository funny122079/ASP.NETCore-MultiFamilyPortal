# Tenant Setup

## Database

Before beginning be sure to setup a database and take note of the database name. You will need this when adding the tenant

## Google Captcha
In order to set up Captcha go to [https://www.google.com/recaptcha/admin](https://www.google.com/recaptcha/admin) and set up a v2 app. If in production you can simply add this to the MultiFamilyPortal app. Take note of the Site Key and Secret which you will need for the Tenant Settings.

## Microsoft Account

A new app will need to be created. This can be done from a personal AAD Tenant or one associated with a Microsoft 365 domain. In production scenarios we prefer to associate it with the client's AAD Tenant. Begin by creating a new application from the Azure Portal. When creating the application be sure to configure it as follows:

- **Supported Account Types:** Accounts in any organizational directory (Any Azure AD directory - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)
- **Redirect Url:** https://{tenantDomain}/signin-microsoft

After creating the app, take note of the **Application (client) ID**. You will need this for the configuration. Next go to the Certificates &amp; Secrets section and create a new application secret. Set the expiration to 12 months so that we know to update this for the service renewal. In Development you can set this to any timeframe. After creating the secret make a note of it along with the Client Id from earlier.

For Production Scenarios be sure to update the Application Branding with the MPN Id: 4652615. This will validate the application as a trusted application.

Additional Reading: [AspNetCore Authentication Docs](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-6.0)

## Google Auth

To begin be sure to log into the [Google Api Console](https://console.cloud.google.com/apis/dashboard).

1. Create a new project. Be sure to set this up for External users.
2. Configure the App Info. Name, Logo, and root domain.
3. No need to configure any scopes, we do not need anything beyond the most basic (OOB) scopes.
4. Add any necessary test users for the Tenant
5. After completing the initial setup and going back to the Dashboard, be sure to go to Domain Verification and verify the domain.
6. Go to Credentials and create a new OAuth Client Id.
  - Add the required Authorized Javascript origins `https://{tenantDomain}`
  - Add the required Authorized Redirect URIs `https://{tenantDomain}/signin-google`

Be sure to take note of the Client Id and the Client Secret.

Additional Reading: [AspNetCore Authentication Docs](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-6.0)

## Production Setup

If working in production, the domain will need to update the DNS Settings to point at the Web App. Be sure to add a new A record with the web app IP Address and TXT record `asuid` with the Custom Domain Verification ID.

Once the A record and TXT record are added, you must update the Web App in the Azure Portal to add the new custom domain.
