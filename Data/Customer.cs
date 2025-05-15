using System.ComponentModel.DataAnnotations;

namespace LLM_Module.Data
{
    public class Customer
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Phone {  get; set; }

        [Required]
        public string Password { get; set; }
    }
}
