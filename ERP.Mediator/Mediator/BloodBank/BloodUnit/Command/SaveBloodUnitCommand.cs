using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.BloodUnit.Command

{

    public class SaveBloodUnitCommand : IRequest<long>

    {

        public long Id { get; set; }

        public long? BloodFridgeId { get; set; }

        public long? BloodRackId { get; set; }

        public string SlotNo { get; set; }

        public int Status { get; set; }

    }

}

