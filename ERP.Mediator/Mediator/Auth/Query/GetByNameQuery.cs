using MediatR;
using ERP.BusinessModels.ResponseVM;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Auth.Query
{

    public class GetByNameQuery : IRequest<List<GetAllUsers>>
    {
        public GetByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}