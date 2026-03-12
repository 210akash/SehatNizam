using MediatR;
using ERP.BusinessModels.ResponseVM;
using System;

namespace ERP.Mediator.Mediator.Auth.Query
{

    public class GetByIdQuery : IRequest<GetAllUsers>
    {
        public GetByIdQuery(Guid UserId)
        {
            this.UserId = UserId;
        }

        public Guid UserId { get; set; }
    }
}