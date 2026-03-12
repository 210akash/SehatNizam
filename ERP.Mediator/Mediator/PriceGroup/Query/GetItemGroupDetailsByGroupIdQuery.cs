using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.PriceGroup.Query
{
    public class GetItemGroupDetailsByGroupIdQuery : IRequest<List<GetItemGroupDetails>>
    {
        public GetItemGroupDetailsByGroupIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}