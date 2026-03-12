using MediatR;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class ApproveShopQuery : IRequest<bool>
    {
        public ApproveShopQuery(long Id, string Remarks)
        {
            this.Id = Id;
            this.Remarks = Remarks;
        }

        public long Id { get; set; }
        public string Remarks { get; set; }
    }
}