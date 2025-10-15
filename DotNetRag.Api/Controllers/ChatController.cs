using DotNetRag.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetRag.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(RagPdfGemini ragService) : ControllerBase
    {
        
        [HttpPost("ask")]
        public IActionResult Ask([FromForm] string question)
        {
            return ragService.AskAsync(question) is Task<string> answer
                ? Ok(new { Answer = answer.Result })
                : BadRequest("Error processing the request.");
        }

        [HttpPost("embedding")]
        public IActionResult Embeddings([FromForm] string question)
        {
            return ragService.GetEmbeddings(question) is Task<string> answer
                ? Ok(new { Answer = answer.Result })
                : BadRequest("Error processing the request.");
        }
    }
}
