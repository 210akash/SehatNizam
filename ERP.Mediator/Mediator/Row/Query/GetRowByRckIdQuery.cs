using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Row.Query
{
    public class GetRowByRackIdQuery : IRequest<List<GetRow>>
    {
        public GetRowByRackIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}