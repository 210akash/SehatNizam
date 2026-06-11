using System;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.CrossMatch.Command
{
    public class SaveBloodCrossMatchCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long BloodRequestId { get; set; }
        public long BloodUnitId { get; set; }
        public DateTime CrossMatchDate { get; set; }
        public int Result { get; set; }
        public string Remarks { get; set; }
    }
}
