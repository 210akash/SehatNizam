using System;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterial.Query
{
    public class ApproveSaleMaterialQuery : IRequest<Tuple<long, string>>
    {
        public ApproveSaleMaterialQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}