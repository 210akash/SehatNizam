using MediatR;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class RejectShopQuery : IRequest<bool>
    {
        public RejectShopQuery(long Id,string Remarks)
        {
            this.Id = Id;
            this.Remarks = Remarks;
        }

        public long Id { get; set; }
        public string Remarks { get; set; }
    }
}