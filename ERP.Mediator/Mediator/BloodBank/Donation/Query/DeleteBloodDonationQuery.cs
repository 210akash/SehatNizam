using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Donation.Query

{

    public class DeleteBloodDonationQuery : IRequest<bool>

    {

        public long Id { get; set; }

        public DeleteBloodDonationQuery(long id) { Id = id; }

    }

}

