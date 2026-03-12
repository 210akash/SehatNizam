//-----------------------------------------------------------------------
// <copyright file="IndividualSensorCommand.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------


using MediatR;
using ERP.BusinessModels.BaseVM;
using System;

namespace ERP.Mediator.Mediator.Error.Command
{
    public class AddErrorCommand : IRequest<bool>
    {
        public string Message { get; set; }

        public string StackTrace { get; set; }
        public Guid? UserId { get; set; }
    }
}