using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Authorize(Policy = PortalPolicy.Blogger)]
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private IBlogContext _context { get; }

        public BlogController(IBlogContext context)
        {
            _context = context;
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var views = await _context.PostListViews
                .OrderByDescending(x => x.Published)
                .ToListAsync();

            return Ok(views);
        }
    }
}
