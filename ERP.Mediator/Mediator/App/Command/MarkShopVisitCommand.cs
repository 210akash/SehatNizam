using MediatR;
using System;

namespace ERP.Mediator.Mediator.App.Command
{
    public class MarkShopVisitCommand : IRequest<long>
    {
        public Guid userId { get; set; }
        public long ShopId { get; set; }
        public bool IsOpen { get; set; }
        public string Comments { get; set; }
        public string PinLocation { get; set; }
        public string ImageSource { get; set; }
    }
}
