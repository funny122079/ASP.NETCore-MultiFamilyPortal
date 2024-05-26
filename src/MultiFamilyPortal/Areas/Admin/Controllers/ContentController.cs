using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Authorize(Policy = PortalPolicy.Blogger)]
    [Area("Admin")]
    [Route("/api/[area]/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private IMFPContext _dbContext { get; }

        public ContentController(IMFPContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetPageContent()
        {
            var content = await _dbContext.CustomContent.ToArrayAsync();

            return Ok(content);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPageContent(string id)
        {
            var content = await _dbContext.CustomContent.FirstOrDefaultAsync(x => x.Id == id);
            if (content is null)
                return NotFound();

            return Ok(content);
        }

        [HttpPost("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody]CustomContent updated)
        {
            var existing = await _dbContext.CustomContent.FirstOrDefaultAsync(x => x.Id == id);
            if (existing is null)
                return NotFound();

            existing.Title = updated.Title;
            existing.HtmlContent = updated.HtmlContent;
            existing.LastUpdated = DateTimeOffset.Now;

            _dbContext.CustomContent.Update(existing);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
