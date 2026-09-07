using CropDeal.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace CropDeal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<ChatController> _logger;

    public ChatController(HttpClient httpClient, IConfiguration config, ILogger<ChatController> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ChatResponse> Chat([FromBody] ChatRequest request)
    {
        try
        {
           
            var apiKey = _config["OpenAI:ApiKey"];

if (string.IsNullOrWhiteSpace(apiKey))
{
    _logger.LogWarning("OpenAI API key not configured, using fallback responses");
    return GetFallbackResponse(request.Message);
}

            var payload = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { 
                        role = "system", 
                        content = "You are a helpful agricultural assistant for CropDeal marketplace. Provide practical farming advice, crop information, and platform guidance. Keep responses concise and helpful." 
                    },
                    new { role = "user", content = request.Message }
                },
                max_tokens = 200,
                temperature = 0.7
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogInformation("Sending request to OpenAI API");
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var result = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation($"OpenAI Response Status: {response.StatusCode}");
            _logger.LogInformation($"OpenAI Response Content: {result.Substring(0, Math.Min(200, result.Length))}...");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"OpenAI API error: {response.StatusCode} - {result}");
                return GetFallbackResponse(request.Message);
            }

            // Check if response is JSON
            if (!result.TrimStart().StartsWith("{"))
            {
                _logger.LogError($"OpenAI returned non-JSON response: {result}");
                return GetFallbackResponse(request.Message);
            }

            using var doc = JsonDocument.Parse(result);
            
            if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var reply = choices[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString()?.Trim();

                if (!string.IsNullOrEmpty(reply))
                {
                    _logger.LogInformation("Successfully got AI response");
                    return new ChatResponse { Reply = reply };
                }
            }

            _logger.LogWarning("No valid response from OpenAI, using fallback");
            return GetFallbackResponse(request.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in chat endpoint, using fallback response");
            return GetFallbackResponse(request.Message);
        }
    }

    private ChatResponse GetFallbackResponse(string message)
    {
        var responses = new Dictionary<string, string>
        {
            {"hello", "Hi! I'm your CropDeal assistant. How can I help you today?"},
            {"hi", "Hello! Welcome to CropDeal. I'm here to help with your farming questions."},
            {"winter", "Winter crops include cabbage, carrots, spinach, kale, radishes, and turnips. These grow well in cool weather."},
            {"summer", "Summer crops include tomatoes, peppers, corn, beans, cucumbers, and squash. Plant after frost danger passes."},
            {"tomato", "Plant tomatoes after the last frost, typically in March-May. Soil should be 60°F or warmer."},
            {"wheat", "Wheat is typically planted in fall (October-November) and harvested in summer (June-July)."},
            {"rice", "Rice needs flooded fields and warm weather. Plant in late spring, harvest in fall."},
            {"corn", "Plant corn when soil is 60°F, typically late April to early June. Needs full sun and rich soil."},
            {"fertilizer", "Use nitrogen for leafy growth, phosphorus for roots, potassium for disease resistance. Test soil first."},
            {"cropdeal", "CropDeal is an agricultural marketplace connecting farmers and buyers for fair crop trading."},
            {"help", "I can help with crop advice, planting schedules, and CropDeal platform questions."}
        };

        var lowerMessage = message.ToLower();
        foreach (var key in responses.Keys)
        {
            if (lowerMessage.Contains(key))
            {
                return new ChatResponse { Reply = responses[key] };
            }
        }

        return new ChatResponse { Reply = "I can help with crop questions, planting advice, and CropDeal platform guidance. What would you like to know?" };
    }
}