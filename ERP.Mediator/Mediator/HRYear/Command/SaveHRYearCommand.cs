using MediatR;
using System;

namespace ERP.Mediator.Mediator.HRYear.Command
{
    public class SaveHRYearCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
