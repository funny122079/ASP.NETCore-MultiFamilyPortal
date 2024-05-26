using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiFamilyPortal.Services;
using MultiFamilyPortal.Themes.Internals;
using SysFile = System.IO.File;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [Route("theme/[controller]")]
    [ApiController]
    public class BrandingController : ControllerBase
    {
        private IWebHostEnvironment _env { get; }
        private IBrandService _brand { get; }

        public BrandingController(IWebHostEnvironment env, IBrandService brand)
        {
            _env = env;
            _brand = brand;
        }

        [HttpGet("logo")]
        public async Task<IActionResult> Logo() => await Get("logo");

        [HttpGet("logo-side")]
        public async Task<IActionResult> LogoSide() => await Get("logo-side");

        [HttpGet("logo-dark")]
        public async Task<IActionResult> LogoDark() => await Get("logo-dark");

        [HttpGet("logo-dark-side")]
        public async Task<IActionResult> LogoDarkSide() => await Get("logo-dark-side");

        [HttpGet("resource")]
        public async Task<IActionResult> Resource(string file, [FromServices]IThemeFactory themeFactory)
        {
            var theme = themeFactory.GetFrontendTheme();
            var resource = theme.Resources.FirstOrDefault(x => x.Name.Equals(file, StringComparison.InvariantCultureIgnoreCase));

            if(resource is null)
                return NotFound();

            return await Get(resource.Name, resource.Path, true);
        }

        private async Task<IActionResult> Get(string name, string defaultFile = null, bool redirect = false)
        {
            var Jpg = $"{name}.jpg";
            var Png = $"{name}.png";
            var Svg = $"{name}.svg";

            if (string.IsNullOrEmpty(defaultFile))
                defaultFile = Path.Combine(_env.WebRootPath, "default-resources", "logo");

            var jpgInfo = FileTypeLookup.GetFileTypeInfo(Jpg);
            var pngInfo = FileTypeLookup.GetFileTypeInfo(Png);
            var svgInfo = FileTypeLookup.GetFileTypeInfo(Svg);

            var brand = await _brand.GetBrandImage(name);
            if(brand.Stream != Stream.Null)
                return File(brand.Stream, brand.MimeType, brand.FileName);

            else if (redirect)
                return Redirect(defaultFile);

            else if (SysFile.Exists(Path.Combine(defaultFile, Png)))
                return PhysicalFile(Path.Combine(defaultFile, Png), pngInfo.MimeType);

            else if (SysFile.Exists(Path.Combine(defaultFile, Svg)))
                return PhysicalFile(Path.Combine(defaultFile, Svg), svgInfo.MimeType);

            else if (SysFile.Exists(Path.Combine(defaultFile, Jpg)))
                return PhysicalFile(Path.Combine(defaultFile, Jpg), jpgInfo.MimeType);

            else
                return NotFound();
        }
    }
}
