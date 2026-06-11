using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Rack.Command

{

    public class SaveRackCommand : IRequest<long>

    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long BloodFridgeId { get; set; }

    }

}

