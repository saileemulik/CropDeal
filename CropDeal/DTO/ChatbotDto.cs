namespace CropDeal.DTO;

public class ChatbotRequestDto
{
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
}

public class ChatbotResponseDto
{
    public string Response { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}