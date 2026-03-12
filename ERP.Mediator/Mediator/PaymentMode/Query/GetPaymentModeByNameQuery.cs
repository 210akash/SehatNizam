using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Query
{
    public class GetPaymentModeByNameQuery : IRequest<List<GetPaymentMode>>
    {
        public GetPaymentModeByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}