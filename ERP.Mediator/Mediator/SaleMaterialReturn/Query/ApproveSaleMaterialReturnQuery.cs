using MediatR;
using System;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Query
{
    public class ApproveSaleMaterialReturnQuery : IRequest<Tuple<long, string>>
    {
        public ApproveSaleMaterialReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}