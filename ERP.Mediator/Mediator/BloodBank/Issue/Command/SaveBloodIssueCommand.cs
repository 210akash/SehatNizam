using System;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Issue.Command
{
    public class SaveBloodIssueCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long BloodRequestId { get; set; }
        public long BloodUnitId { get; set; }
        public long? BloodCrossMatchId { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuedTo { get; set; }
        public string Remarks { get; set; }
    }
}
