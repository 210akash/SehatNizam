using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Issue.Query
{
    public class GetAllBloodIssueQuery : IRequest<Tuple<IEnumerable<GetBloodIssue>, long>>
    {
        public long? BloodRequestId { get; set; }
        public string RequestCode { get; set; }
        public string IssuedTo { get; set; }
        public PagingData PagingData { get; set; }
    }
}
