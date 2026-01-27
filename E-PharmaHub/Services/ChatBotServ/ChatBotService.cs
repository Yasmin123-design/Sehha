using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace E_PharmaHub.Services.ChatBotServ
{
    public class ChatBotService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ChatBotService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> AskAsync(string userMessage)
        {
            var apiKey = _config["OpenAI:ApiKey"];

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                new { role = "system", content = "You are a healthcare assistant chatbot." },
                new { role = "user", content = userMessage }
            }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                content
            );

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString();
        }
    }
}
