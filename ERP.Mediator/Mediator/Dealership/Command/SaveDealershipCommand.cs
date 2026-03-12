using ERP.BusinessModels.ParameterVM;
using ERP.Entities.Models;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Dealership.Command
{
    public class SaveDealershipCommand : IRequest<long>
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
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
        public long? TerritoryId { get; set; }
        public long DealershipTypeId { get; set; }

        public List<ImageUploadModel> DealershipImages { get; set; }
    }
}
