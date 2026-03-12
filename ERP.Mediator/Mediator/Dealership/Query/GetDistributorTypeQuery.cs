using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Dealership.Query
{
    public class GetDistributorTypeQuery : IRequest<List<GetDistributorType>>
    {
    }
}
