using System.ComponentModel.DataAnnotations;

namespace LLM_Module.Data
{
    public class Order
    {
        [Required]
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime Deadline { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public IFormFile? File { get; set; }
    }
}
