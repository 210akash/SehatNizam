using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Query
{
    public class DeleteVehicleQuery : IRequest<long>
    {
        public DeleteVehicleQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}