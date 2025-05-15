using System;
using System.ComponentModel.DataAnnotations;

namespace LLM_Module.Data
{
    public class Support
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Topic { get; set; }

        public int? OrderId { get; set; }

        [Required]
        public string Message { get; set; }

        public IFormFile? File { get; set; }
    }
}
