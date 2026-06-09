using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.IPD.AdmissionPackage.Command
{
    public class SaveAdmissionPackageMasterCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual List<SaveAdmissionPackageDetailCommand> AdmissionPackageDetail { get; set; }
    }

    public class SaveAdmissionPackageDetailCommand
    {
        public long Id { get; set; }
        public long AdmissionPackageMasterId { get; set; }
        public long ServiceId { get; set; }
    }
}
