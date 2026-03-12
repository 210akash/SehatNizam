using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.RetailOrder.Query
{
    public class GetKCItemsByDistributorRetailQuery : IRequest<List<GetItemStock>>
    {
    }
}
