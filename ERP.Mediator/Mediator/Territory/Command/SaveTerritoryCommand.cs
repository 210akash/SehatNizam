using ERP.Entities.Models;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Territory.Command
{
    public class SaveTerritoryCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
        public string SaleModel { get; set; }
        public long AreaId { get; set; }
    }
}
