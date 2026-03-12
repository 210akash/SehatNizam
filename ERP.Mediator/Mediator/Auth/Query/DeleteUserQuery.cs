using MediatR;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class DeleteUserQuery : IRequest<long>
    {
        public DeleteUserQuery(string Id)
        {
            this.Id = Id;
        }

        public string Id { get; set; }
    }
}