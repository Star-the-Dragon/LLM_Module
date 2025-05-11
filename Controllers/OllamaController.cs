using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LLM_Module.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OllamaController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public OllamaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public class PromptRequest
        {
            public string Prompt { get; set; }
        }

        [HttpPost("chat")]
        public async Task<IActionResult> ChatWithOllama([FromBody] PromptRequest request)
        {
            var ollamaPayload = new
            {
                //model = "deepseek-r1:1.5b",
                model = "gemma3:4b",
                prompt = "Ты помощник на сайте посвященному размещению производственных заказов. Выдели наиболее важные сведения из полученного ТЗ." + request.Prompt,
                stream = false
            };

            var content = new StringContent(
                JsonSerializer.Serialize(ollamaPayload),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Ошибка обращения к LLM");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var message = doc.RootElement.GetProperty("response").GetString();

            return Ok(new { reply = message });
        }
    }
}
