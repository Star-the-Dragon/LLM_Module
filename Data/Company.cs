using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LLM_Module.Data
{
    public class Company
    {
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string Address { get; set; }
        public string Description { get; set; }

        public List<Capability>? Capabilities { get; set; }
    }
}
