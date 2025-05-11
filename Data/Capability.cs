using LLM_Module.Data;

namespace LLM_Module.Data
{
    public class Capability
    {
        public int CapabilityId { get; set; }
        public int CompanyId { get; set; }
        public string Material { get; set; }
        public string Process { get; set; }
        public int MaxDiameter { get; set; }
        public int MaxLength { get; set; }
        public string Notes { get; set; }

        public Company Company { get; set; }
    }
}
