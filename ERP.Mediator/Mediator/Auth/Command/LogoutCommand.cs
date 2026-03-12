//-----------------------------------------------------------------------
// <copyright file="LogoutCommand.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using ERP.BusinessModels.ResponseVM;


namespace ERP.Mediator.Mediator.Auth.Command
{
    /// <summary>
    /// Declaration of Logout Command class.
    /// </summary>
    public class LogoutCommand : IRequest<IdentityResult>
    {
    }
}