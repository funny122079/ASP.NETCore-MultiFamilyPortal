using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class FavIconController : ControllerBase
    {
        private IBrandService _brand { get; }

        public FavIconController(IBrandService brand)
        {
            _brand = brand;
        }

        [HttpGet("/favicon.ico")]
        public async Task<IActionResult> GetFavIconAsync() =>
            await GetImageAsync("favicon.ico");

        [HttpGet("/favicon-16x16.png")]
        public async Task<IActionResult> GetFavIcon16Async() =>
            await GetImageAsync("favicon-16x16.png");

        [HttpGet("/favicon-32x32.png")]
        public async Task<IActionResult> GetFavIcon32Async() =>
            await GetImageAsync("favicon-32x32.png");

        [HttpGet("/android-chrome-192x192.png")]
        public async Task<IActionResult> AndroidChrome192Async() =>
            await GetImageAsync("android-chrome-192x192.png");

        [HttpGet("/android-chrome-512x512.png")]
        public async Task<IActionResult> AndroidChrome512Async() =>
            await GetImageAsync("android-chrome-512x512.png");

        [HttpGet("/apple-touch-icon.png")]
        public async Task<IActionResult> GetAppleTouchAsync() =>
            await GetImageAsync("apple-touch-icon.png");

        [HttpGet("/site.webmanifest")]
        public async Task<IActionResult> SiteWebManifest([FromServices] IMFPContext db)
        {
            var title = await db.GetSettingAsync<string>(PortalSetting.SiteTitle);
            var legalName = await db.GetSettingAsync<string>(PortalSetting.LegalBusinessName);

            if (string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(legalName))
            {
                title = legalName;
            }
            else if (!string.IsNullOrEmpty(title) && string.IsNullOrEmpty(legalName))
            {
                legalName = title;
            }
            else
            {
                title = legalName = "Multi-Family Portal";
            }

            var manifest = new WebManifest
            {
                BackgroundColor = await db.GetSettingAsync(PortalSetting.BackgroundColor, "#ffffff"),
                ThemeColor = await db.GetSettingAsync(PortalSetting.ThemeColor, "#ffffff"),
                Name = legalName,
                ShortName = title,
                Icons = new[]
                {
                    new Icon { Source = "/android-chrome-192x192.png", Sizes = "192x192" },
                    new Icon { Source = "/android-chrome-512x512.png", Sizes = "512x512" }
                }
            };
            var json = JsonSerializer.Serialize(manifest);
            var bytes = Encoding.Default.GetBytes(json);
            return File(bytes, "application/json", "site.webmanifest");
        }

        private async Task<IActionResult> GetImageAsync(string name)
        {
            using var stream = await _brand.GetIcon(name);
            if (stream is null || stream == Stream.Null)
                return NotFound();

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var data = memoryStream.ToArray();

            var typeInfo = FileTypeLookup.GetFileTypeInfo(name);
            return File(data, typeInfo.MimeType, name);
        }

        private record WebManifest
        {
            [JsonPropertyName("name")]
            public string Name { get; init; } = default!;

            [JsonPropertyName("short_name")]
            public string ShortName { get; init; } = default!;

            [JsonPropertyName("theme_color")]
            public string ThemeColor { get; init; } = default!;

            [JsonPropertyName("background_color")]
            public string BackgroundColor { get; init; } = default!;

            [JsonPropertyName("display")]
            public string Display => "standalone";

            [JsonPropertyName("icons")]
            public IEnumerable<Icon> Icons { get; init; } = default!;
        }

        private record Icon
        {
            [JsonPropertyName("src")]
            public string Source { get; init; } = default!;

            [JsonPropertyName("sizes")]
            public string Sizes { get; init; } = default!;

            [JsonPropertyName("type")]
            public string Type => "image/png";
        }
    }
}
