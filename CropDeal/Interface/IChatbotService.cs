using CropDeal.DTO;

namespace CropDeal.Interface;

public interface IChatbotService
{
    Task<ChatbotResponseDto> GetResponseAsync(ChatbotRequestDto request);
}