using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Query
{
    public class GetAllDeliveryTermsQuery : IRequest<Tuple<IEnumerable<GetDeliveryTerms>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}