using ERP.Entities.Models;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Route.Command
{
    public class SaveRouteCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        //public string VisitDay { get; set; }
        public long TerritoryId { get; set; }
    }
}
