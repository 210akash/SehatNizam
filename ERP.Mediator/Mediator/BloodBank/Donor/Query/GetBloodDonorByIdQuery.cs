using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Donor.Query
{
    public class GetBloodDonorByIdQuery : IRequest<GetBloodDonor>
    {
        public long Id { get; set; }
        public GetBloodDonorByIdQuery(long id) { Id = id; }
    }
}
