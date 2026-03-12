using MediatR;

namespace ERP.Mediator.Mediator.ShopType.Command
{
    public class SaveShopTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
