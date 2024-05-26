using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [Route("/api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private IMFPContext _context { get; }
        private ISiteInfo _siteInfo { get; }

        public ContentController(IMFPContext context, ISiteInfo siteInfo)
        {
            _context = context;
            _siteInfo = siteInfo;
        }

        [HttpGet("{contentId}")]
        public async Task<IActionResult> GetContent(string contentId)
        {
            var content = await _context.CustomContent.FirstOrDefaultAsync(x => x.Id == contentId);

            if (content is null)
                return NotFound();

            content.HtmlContent = Regex.Replace(content.HtmlContent, "{Address}", _siteInfo.Address);
            content.HtmlContent = Regex.Replace(content.HtmlContent, "{City}", _siteInfo.City);
            content.HtmlContent = Regex.Replace(content.HtmlContent, "{State}", _siteInfo.State);
            content.HtmlContent = Regex.Replace(content.HtmlContent, "{PostalCode}", _siteInfo.PostalCode);
            content.HtmlContent = Regex.Replace(content.HtmlContent, "{PublicEmail}", _siteInfo.PublicEmail);
            content.HtmlContent = Regex.Replace(content.HtmlContent, "{LegalBusinessName}", _siteInfo.LegalBusinessName);

            return Ok(content);
        }
    }
}
