using ERP.Entities.Models;

namespace ERP.Entities.Models
{
    public partial class Templates : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }
}
