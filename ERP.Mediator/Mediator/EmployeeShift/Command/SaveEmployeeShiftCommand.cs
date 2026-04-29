using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ERP.Mediator.Mediator.EmployeeShift.Command
{
    public class SaveEmployeeShiftCommand : IRequest<long>
    {
        public long Id { get; set; }

        [MaxLength(5)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public bool IsDualDate { get; set; } = false;
    }
}
