using DotNetRag.Api.Models;
using DotNetRag.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetRag.Api.Controllers
{
    [ApiController]                     
    [Route("api/[controller]")]
    public class FileController(ITempFileCache _tempFileCache) : ControllerBase
    {
      

        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultiple([FromForm] List<IFormFile> files, [FromForm] string sessionId)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

            foreach (var file in files)
            {
                if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    return BadRequest($"File '{file.FileName}' is not a PDF.");

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
              
                var content = PDFService.ExtractContentFromPDF(file.Name);
                var entry = new TempFileEntry
                {
                    FileName = file.FileName,
                    Content = content,
                    UploadedAt = DateTime.UtcNow
                };
                await _tempFileCache.AddFileAsync(sessionId, entry);
            }

            return Ok(new { files.Count, Message = "Files uploaded and processed successfully." });
        }
    }
}
