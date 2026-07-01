using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class ServiceType : BaseEntity
    {
        public string Name { get; set; }           // OPD, IPD, LAB
        public virtual List<ServiceAccount> ServiceAccounts { get; set; }
    }
}
