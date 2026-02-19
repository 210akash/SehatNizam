using MediatR;

namespace ERP.Mediator.Mediator.Vendor.Command
{
    public class SaveVendorCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string NTN { get; set; }
        public string TermsConditions { get; set; }
        public int CreditDays { get; set; }
        public long? VendorTypeId { get; set; }
        public long CompanyId { get; set; }
    }
}
