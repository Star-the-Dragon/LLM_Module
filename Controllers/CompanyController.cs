using LLM_Module.Data;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace LLM_Module.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : Controller
    {
        private readonly string _connectionString;

        public CompanyController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<IActionResult> PostOrder([FromForm] Company model)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO public.companies 
                    (""Name"", ""Surname"", ""Email"", ""Phone"", ""Password"", ""CompanyName"", ""Address"", ""Description"")
                    VALUES 
                    (@name, @surname, @email, @phone, @password, @companyname, @address, @description)", conn);

                cmd.Parameters.AddWithValue("name", model.Name);
                cmd.Parameters.AddWithValue("surname", model.Surname);
                cmd.Parameters.AddWithValue("email", model.Email);
                cmd.Parameters.AddWithValue("phone", model.Phone);
                cmd.Parameters.AddWithValue("password", model.Password);
                cmd.Parameters.AddWithValue("companyname", model.CompanyName);
                cmd.Parameters.AddWithValue("address", model.Address);
                cmd.Parameters.AddWithValue("description", model.Description);

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
