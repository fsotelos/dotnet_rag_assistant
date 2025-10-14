using DotNetRag.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetRag.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(RagService ragService) : ControllerBase
    {
        
        [HttpPost("ask")]
        public IActionResult Ask([FromForm] string question)
        {
            return ragService.MakeQuestion(question) is Task<string> answer
                ? Ok(new { Answer = answer.Result })
                : BadRequest("Error processing the request.");
        }
    }
}
