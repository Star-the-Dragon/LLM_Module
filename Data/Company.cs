using System.Collections.Generic;

namespace LLM_Module.Data
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }

        public List<Capability> Capabilities { get; set; }
    }
}
