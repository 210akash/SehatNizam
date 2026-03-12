using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Dealership.Query
{
    public class GetAllDealershipListQuery : IRequest<Tuple<IEnumerable<GetDealership>, long>>
    {
        public long RegionId { get; set; }
        public long ZoneId { get; set; }
        public long AreaId { get; set; }
        public long TerritoryId { get; set; }
        public bool? IsActive { get; set; }
        public long DealershipTypeId { get; set; }
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
