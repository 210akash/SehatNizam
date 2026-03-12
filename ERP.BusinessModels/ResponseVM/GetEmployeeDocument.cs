using ERP.Entities.Models;
using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeDocument
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long EmployeeDocumentTypeId { get; set; }
        public GetEmployeeDocumentType EmployeeDocumentType { get; set; }

        public Guid EmployeeId { get; set; }
        public GetUsers Employee { get; set; }

        public GetUser CreatedBy { get; set; }
    }
}
