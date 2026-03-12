using ERP.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetDealership
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string SecondaryPhoneNo { get; set; }
        public string Address { get; set; }
        public string PinLocation { get; set; }
        public string Landmark { get; set; }
        public string CNIC { get; set; }
        public string NTN { get; set; }
        public string OwnerName { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long? TerritoryId { get; set; }
        public GetTerritory Territory { get; set; }

        [NotMapped]
        public virtual GetUser User { get; set; }

        public long DealershipTypeId { get; set; }
        public GetDealershipType DealershipType { get; set; }

        public List<GetAttachments> Attachments { get; set; }
        public List<GetAccountGroup> AccountGroup { get; set; }
    }

    public class GetDealershipLite
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string PinLocation { get; set; }
        public string TerritoryName { get; set; }
        public string ZoneName { get; set; }
        public string AreaName { get; set; }
        public string RegionName { get; set; }
    }
}
