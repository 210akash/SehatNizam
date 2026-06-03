using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Referrer.Query
{
    public class GetAllReferrerQuery : IRequest<Tuple<IEnumerable<GetReferrer>, long>>
    {
        public string Name { get; set; }
        public string Hospital { get; set; }
        public string PhoneNo { get; set; }
        public PagingData PagingData { get; set; }
    }
}