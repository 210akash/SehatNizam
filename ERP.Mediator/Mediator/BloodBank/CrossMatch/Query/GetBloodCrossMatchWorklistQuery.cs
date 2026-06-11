using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.CrossMatch.Query
{
    public class GetBloodCrossMatchWorklistQuery : IRequest<Tuple<IEnumerable<GetBloodCrossMatchWorklist>, long>>
    {
        public string RequestCode { get; set; }
        public PagingData PagingData { get; set; }
    }
}
