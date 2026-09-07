# CropDeal Chatbot Integration Guide

## Overview
A small AI-powered chatbot has been integrated into your CropDeal application using OpenAI's GPT-3.5-turbo model.

## Setup Instructions

### 1. Get OpenAI API Key
1. Visit [OpenAI Platform](https://platform.openai.com/)
2. Create an account or sign in
3. Go to API Keys section
4. Create a new API key
5. Copy the key (starts with `sk-`)

### 2. Configure API Key
Update your `appsettings.json` file:
```json
"OpenAI": {
  "ApiKey": "your-actual-openai-api-key-here",
  "Model": "gpt-3.5-turbo"
}
```

### 3. Install Dependencies
Run in your backend project directory:
```bash
dotnet restore
```

### 4. Update Frontend API URL
In `cropfront/src/app/services/chatbot.service.ts`, update the API URL if needed:
```typescript
private apiUrl = 'https://localhost:7035/api/chatbot'; // Update port if different
```

### 5. Build and Run
1. Backend: `dotnet run`
2. Frontend: `ng serve`

## Features
- **Floating Chat Button**: Bottom-right corner of the screen
- **Context-Aware**: Knows about CropDeal platform
- **Responsive Design**: Works on mobile and desktop
- **Real-time Chat**: Instant responses from AI
- **Professional UI**: Matches your app's design

## Usage
1. Click the green chat button (💬) in bottom-right corner
2. Type your question about crops, trading, platform usage, etc.
3. Press Enter or click send button
4. Get instant AI-powered responses

## Customization Options

### Change AI Model
In `appsettings.json`:
```json
"OpenAI": {
  "Model": "gpt-4" // More advanced but costs more
}
```

### Customize System Prompt
Edit `Repository/ChatbotService.cs` line 19-22 to change the AI's behavior and knowledge.

### Styling
Modify `chatbot.component.css` to match your brand colors and design.

## Cost Considerations
- GPT-3.5-turbo: ~$0.002 per 1K tokens
- GPT-4: ~$0.03 per 1K tokens
- Average conversation: 100-500 tokens
- Monthly cost for moderate usage: $5-20

## Alternative AI Services
You can easily switch to other providers by modifying the `ChatbotService`:
- **Azure OpenAI**: Enterprise-grade with compliance
- **Google Gemini**: Cost-effective alternative
- **Anthropic Claude**: Good for conversational AI

## Security Notes
- Never commit API keys to version control
- Use environment variables in production
- Consider rate limiting for production use
- Monitor API usage and costs

## Troubleshooting
1. **API Key Error**: Verify key is correct and has credits
2. **CORS Issues**: Check API URL and CORS settings
3. **No Response**: Check network connectivity and API status
4. **Styling Issues**: Ensure Font Awesome is loaded

## Support
For issues or customizations, check:
- OpenAI API documentation
- Angular HttpClient documentation
- ASP.NET Core Web API documentation