using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Query
{
    public class GetPaymentModeByCompanyQuery : IRequest<List<GetPaymentMode>>
    {
        public GetPaymentModeByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}