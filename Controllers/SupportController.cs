using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LLM_Module.Data;
using Npgsql;

namespace LLM_Module.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportController : ControllerBase
    {
        private readonly string _connectionString;

        public SupportController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<IActionResult> PostSupport([FromForm] Support model)
        {
            try
            {
                byte[] fileBytes = null;

                if (model.File != null && model.File.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await model.File.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(@"
                INSERT INTO public.support (""Name"", ""Email"", ""Topic"", ""OrderId"", ""Message"", ""File"")
                VALUES (@name, @email, @topic, @orderId, @message, @file)", conn);


                cmd.Parameters.AddWithValue("name", model.Name);
                cmd.Parameters.AddWithValue("email", model.Email);
                cmd.Parameters.AddWithValue("topic", model.Topic);
                cmd.Parameters.AddWithValue("orderId", (object?)model.OrderId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("message", model.Message);
                cmd.Parameters.AddWithValue("file", (object?)fileBytes ?? DBNull.Value);

                Console.WriteLine($"Name: {model.Name}");
                Console.WriteLine($"Email: {model.Email}");
                Console.WriteLine($"Topic: {model.Topic}");
                Console.WriteLine($"OrderId: {model.OrderId}");
                Console.WriteLine($"Message: {model.Message}");


                await cmd.ExecuteNonQueryAsync();

                return Ok(new { message = "Запрос сохранён в базе данных." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
