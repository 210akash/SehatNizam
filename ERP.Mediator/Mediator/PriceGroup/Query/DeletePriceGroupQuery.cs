using MediatR;

namespace ERP.Mediator.Mediator.PriceGroup.Query
{
    public class DeletePriceGroupQuery : IRequest<long>
    {
        public DeletePriceGroupQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}