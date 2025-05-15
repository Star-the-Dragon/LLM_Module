using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using LLM_Module.Data;

namespace LLM_Module.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly string _connectionString;

        public OrderController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<IActionResult> PostOrder([FromForm] Order model)
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
                    INSERT INTO public.orders 
                    (""CompanyId"", ""CustomerId"", ""Name"", ""Description"", ""Quantity"", ""Deadline"", 
                     ""CustomerName"", ""CustomerPhone"", ""CustomerEmail"", ""Files"")
                    VALUES 
                    (@companyId, @customerId, @name, @description, @quantity, @deadline, 
                     @customerName, @customerPhone, @customerEmail, @files)", conn);

                cmd.Parameters.AddWithValue("companyId", model.CompanyId);
                cmd.Parameters.AddWithValue("customerId", model.CustomerId);
                cmd.Parameters.AddWithValue("name", model.Name);
                cmd.Parameters.AddWithValue("description", model.Description);
                cmd.Parameters.AddWithValue("quantity", model.Quantity);
                cmd.Parameters.AddWithValue("deadline", model.Deadline);
                cmd.Parameters.AddWithValue("customerName", model.CustomerName);
                cmd.Parameters.AddWithValue("customerPhone", model.CustomerPhone);
                cmd.Parameters.AddWithValue("customerEmail", model.CustomerEmail);
                cmd.Parameters.AddWithValue("files", (object?)fileBytes ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync();

                return Ok(new { message = "Заказ сохранён в базе данных." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
