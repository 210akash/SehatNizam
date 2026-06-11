using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.CrossMatch.Query
{
    public class GetAllBloodCrossMatchQuery : IRequest<Tuple<IEnumerable<GetBloodCrossMatch>, long>>
    {
        public long? BloodRequestId { get; set; }
        public PagingData PagingData { get; set; }
    }
}
