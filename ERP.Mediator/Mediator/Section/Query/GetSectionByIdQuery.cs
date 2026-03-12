using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Section.Query
{
    public class GetSectionByRowIdQuery  : IRequest<List<GetSection>>
    {
        public GetSectionByRowIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}