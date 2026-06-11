using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Donor.Query

{

    public class DeleteBloodDonorQuery : IRequest<bool>

    {

        public long Id { get; set; }

        public DeleteBloodDonorQuery(long id) { Id = id; }

    }

}

