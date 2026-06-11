using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Issue.Query
{
    public class GetBloodIssueWorklistQuery : IRequest<Tuple<IEnumerable<GetBloodIssueWorklist>, long>>
    {
        public string RequestCode { get; set; }
        public PagingData PagingData { get; set; }
    }
}
