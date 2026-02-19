using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Company : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string NTN { get; set; }
        public string Phone { get; set; }
        public string Logo { get; set; }
        public string Color { get; set; }
        public virtual List<Department> Departments { get; set; }
        public virtual List<Vendor> Vendors { get; set; }
        public virtual List<Category> Categories { get; set; }
        public virtual List<SubCategory> SubCategories { get; set; }
        public virtual List<ItemType> ItemTypes { get; set; }
        public virtual List<Item> Item { get; set; }
    }
}
