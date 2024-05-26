using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Dtos.Portfolio;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Authorize(Policy = PortalPolicy.Underwriter)]
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private IMFPContext _dbContext { get; }

        public PortfolioController(IMFPContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetPortfolioProperties()
        {
            var summary = await _dbContext.AssetsUnderManagement
                .Select(x => new PortfolioSummary
                {
                    City = x.City,
                    Highlighted = x.Highlighted,
                    Id = x.Id,
                    InvestorState = x.InvestorState,
                    Name = x.Name,
                    PurchasePrice = x.PurchasePrice,
                    State = x.State,
                    Status = x.Status,
                    Units = x.Units,
                })
                .ToArrayAsync();

            return Ok(summary);
        }
    }
}
