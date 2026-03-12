using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Account.Query
{
    public class GetGroupAccountQuery : IRequest<List<GetAccount>>
    {
        public GetGroupAccountQuery()
        {
        }
    }
}