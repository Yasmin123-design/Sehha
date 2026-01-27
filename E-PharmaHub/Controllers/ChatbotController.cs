using E_PharmaHub.Dtos;
using E_PharmaHub.Services.ChatBotServ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly ChatBotService _chatBotService;

        public ChatbotController(ChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
        {
            var reply = await _chatBotService.AskAsync(request.Message);
            return Ok(new { reply });
        }
    }
}
