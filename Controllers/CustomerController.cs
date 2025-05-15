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
    public class CustomerController : ControllerBase
    {
        private readonly string _connectionString;

        public CustomerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<IActionResult> PostOrder([FromForm] Customer model)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO public.customers 
                    (""Name"", ""Surname"", ""Email"", ""Phone"", ""Password"")
                    VALUES 
                    (@name, @surname, @email, @phone, @password)", conn);

                cmd.Parameters.AddWithValue("name", model.Name);
                cmd.Parameters.AddWithValue("surname", model.Surname);
                cmd.Parameters.AddWithValue("email", model.Email);
                cmd.Parameters.AddWithValue("phone", model.Phone);
                cmd.Parameters.AddWithValue("password", model.Password);

                await cmd.ExecuteNonQueryAsync();

                return Ok(new { message = "Регистрация успешна." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
