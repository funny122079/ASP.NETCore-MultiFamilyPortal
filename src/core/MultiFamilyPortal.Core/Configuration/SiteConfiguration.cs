using AvantiPoint.FileStorage.AzureBlobStorage;

namespace MultiFamilyPortal.Configuration
{
    internal class SiteConfiguration
    {
        public string PostmarkApiKey { get; set; }

        public GoogleCaptchaOptions Captcha { get; set; }

        public AzureBlobStorageOptions Storage { get; set; }
    }
}
