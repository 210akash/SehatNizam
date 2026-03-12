using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Account.Query
{
    public class GetAccountByAccountFlowQuery : IRequest<List<GetAccountByAccountFlow>>
    {
        public GetAccountByAccountFlowQuery(long AccountFlowId)
        {
            this.AccountFlowId = AccountFlowId;
        }

        public long AccountFlowId { get; set; }
    }
}
