using DotNetRag.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetRag.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(GeminiAIService _geminiAIService) : ControllerBase
    {
        
        [HttpPost("ask")]
        public IActionResult Ask([FromQuery] string prompt)
        {
            return _geminiAIService.GenerateContentAsync(prompt) is Task<string> answer
                ? Ok(new { Answer = answer.Result })
                : BadRequest("Error processing the request.");
        }
    }
}
