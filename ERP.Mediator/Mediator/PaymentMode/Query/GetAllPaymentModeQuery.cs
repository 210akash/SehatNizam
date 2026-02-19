using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Query
{
    public class GetAllPaymentModeQuery : IRequest<Tuple<IEnumerable<GetPaymentMode>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}