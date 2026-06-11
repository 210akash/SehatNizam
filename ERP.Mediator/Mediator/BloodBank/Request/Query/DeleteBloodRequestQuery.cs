using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Request.Query

{

    public class DeleteBloodRequestQuery : IRequest<bool>

    {

        public long Id { get; set; }

        public DeleteBloodRequestQuery(long id) { Id = id; }

    }

}

