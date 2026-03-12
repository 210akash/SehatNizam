using System;

namespace ERP.Entities.Models
{
    public class EmployeeDocument : BaseEntity
    {
        public string Name { get; set; }

        public long EmployeeDocumentTypeId { get; set; }
        public virtual EmployeeDocumentType EmployeeDocumentType { get; set; }

        public Guid EmployeeId { get; set; }
        public virtual AspNetUsers Employee { get; set; }
    }
}
