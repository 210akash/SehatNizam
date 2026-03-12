using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class VerifyShopByIdQuery : IRequest<long>
    {
        public VerifyShopByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
