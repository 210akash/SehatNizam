using MediatR;
using System;

namespace ERP.Mediator.Mediator.IPD.AdmissionServices.Command
{
    public class ConfirmAdmissionServiceCommand : IRequest<Tuple<long, string>>
    {
        public long Id { get; set; }
    }
}