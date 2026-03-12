using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Route.Command
{
    public class AddShopsRouteFrequencyCommand : IRequest<long>
    {
        public long RouteId { get; set; }
        public List<RouteFrequencyList> RouteFrequencyList { get; set; }
    }

    public class RouteFrequencyList
    {
        public long ShopId { get; set; }
        public DaysOfWeek Schedule { get; set; }
    }

    public class DaysOfWeek
    {
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}
