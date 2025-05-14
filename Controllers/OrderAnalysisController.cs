using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using LLM_Module.Data;

namespace LLM_Module.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppDbContext _context;

        public OrderController(IHttpClientFactory httpClientFactory, AppDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        public class OrderRequest
        {
            public string Description { get; set; }
            public int Quantity { get; set; }
            public string Deadline { get; set; }
        }

        [HttpPost("match")]
        public async Task<IActionResult> MatchCompanies([FromBody] OrderRequest request)
        {
            var companies = await _context.Companies
                .Include(c => c.Capabilities)
                .ToListAsync();

            var companyData = companies.Select(c => new
            {
                c.CompanyId,
                c.Name,
                c.Location,
                c.Description,
                c.Email,
                c.Phone,
                Capabilities = c.Capabilities.Select(cap => new
                {
                    cap.Material,
                    cap.Process,
                    cap.MaxDiameter,
                    cap.MaxLength,
                    cap.Notes
                })
            });

            var llmPrompt = $@"
Ты эксперт по подбору производственных исполнителей. 
На вход ты получаешь описание заказа и список компаний с их возможностями. 
Твоя задача — выбрать 3–5 наиболее подходящих компаний. 
Основания для выбора: соответствие материалу, процессу, размеру, объёму, сроку. 
Формат ответа: JSON-массив объектов с полями: companyId, причинаВыбора.

Описание заказа:
{request.Description}

Количество изделий: {request.Quantity}
Срок: {request.Deadline}

Компании:
{JsonSerializer.Serialize(companyData)}
";

            var client = _httpClientFactory.CreateClient();
            var ollamaPayload = new
            {
                model = "gemma3:4b",
                prompt = llmPrompt,
                stream = false
            };

            var content = new StringContent(JsonSerializer.Serialize(ollamaPayload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:11434/api/generate", content);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Ошибка обращения к Ollama");

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var modelOutput = doc.RootElement.GetProperty("response").GetString();

            // Парсинг JSON из ответа модели
            try
            {
                var cleanedOutput = modelOutput
                    .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("```", "")
                    .Trim();

                var selectedCompanies = JsonSerializer.Deserialize<List<MatchedCompany>>(cleanedOutput);
                return Ok(selectedCompanies);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка десериализации: " + ex.Message + "\nModel output: " + modelOutput);
            }
        }

        public class MatchedCompany
        {
            public int CompanyId { get; set; }
            public string Reason { get; set; }
        }
    }
}
