using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Donation.Query
{
    public class GetBloodDonationByIdQuery : IRequest<GetBloodDonation>
    {
        public long Id { get; set; }
        public GetBloodDonationByIdQuery(long id) { Id = id; }
    }
}
