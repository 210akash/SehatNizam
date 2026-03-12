using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.EmployeeDevice.Command
{
    public class SaveEmployeeDeviceCommand : IRequest<Tuple<long, string>>
    {
        public Guid EmployeeId { get; set; }

        public List<EmployeeDevices> EmployeeDevices { get; set; }
    }

    public class EmployeeDevices
    {
        public long Id { get; set; }
        public long DeviceId { get; set; }
        public string EnrollmentNo { get; set; }
    }
}
