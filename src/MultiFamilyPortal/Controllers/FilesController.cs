using AvantiPoint.FileStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private IStorageService _storage { get; }
        private IMFPContext _dbContext { get; }

        public FilesController(IStorageService storage, IMFPContext dbContext)
        {
            _storage = storage;
            _dbContext = dbContext;
        }

        [HttpGet("property/{propertyId:guid}/file/{fileId:guid}")]
        public async Task<IActionResult> GetFile(Guid propertyId, Guid fileId)
        {
            var file = await _dbContext.UnderwritingProspectFiles.FirstOrDefaultAsync(x => x.PropertyId == propertyId && x.Id == fileId);

            if(file is null)
                return NotFound();

            var path = Path.Combine("underwriting", $"{propertyId}", $"{fileId}{Path.GetExtension(file.Name)}");
            using var fileStream = await _storage.GetAsync(path);
            if (fileStream is null || fileStream == Stream.Null || fileStream.Length == 0)
                return NotFound();

            using var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            var data = memoryStream.ToArray();
            var typeInfo = FileTypeLookup.GetFileTypeInfo(file.Name);
            return File(data, typeInfo.MimeType, file.Name);
        }
    }
}
