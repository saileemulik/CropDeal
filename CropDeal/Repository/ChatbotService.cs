using CropDeal.DTO;
using CropDeal.Interface;
using OpenAI.Chat;

namespace CropDeal.Repository;

public class ChatbotService : IChatbotService
{
    private readonly ChatClient _chatClient;
    private readonly IConfiguration _configuration;

    public ChatbotService(IConfiguration configuration)
    {
        _configuration = configuration;
        var apiKey = _configuration["OpenAI:ApiKey"];
        var model = _configuration["OpenAI:Model"] ?? "gpt-3.5-turbo";
        _chatClient = new ChatClient(model, apiKey);
    }

    public async Task<ChatbotResponseDto> GetResponseAsync(ChatbotRequestDto request)
    {
        try
        {
            var systemPrompt = @"You are a helpful assistant for CropDeal, an agricultural marketplace platform. 
                               You help farmers and dealers with questions about crop trading, pricing, listings, 
                               payments, and general platform usage. Keep responses concise and helpful.";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(request.Message)
            };

            var completion = await _chatClient.CompleteChatAsync(messages);
            
            return new ChatbotResponseDto
            {
                Response = completion.Value.Content[0].Text,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new ChatbotResponseDto
            {
                Response = "I'm sorry, I'm having trouble processing your request right now. Please try again later.",
                Timestamp = DateTime.UtcNow
            };
        }
    }
}