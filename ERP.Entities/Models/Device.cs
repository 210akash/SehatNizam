using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Device : BaseEntity
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNo { get; set; }

        public int Port { get; set; }

        public string IPAddress { get; set; }

        public string ConnectionStatus { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual List<EmployeeDevice> EmployeeDevices { get; set; }
    }
}
