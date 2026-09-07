using CropDeal.DTO;
using CropDeal.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CropDeal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotService _chatbotService;

    public ChatbotController(IChatbotService chatbotService)
    {
        _chatbotService = chatbotService;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatbotResponseDto>> Chat([FromBody] ChatbotRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty");
        }

        var response = await _chatbotService.GetResponseAsync(request);
        return Ok(response);
    }
}