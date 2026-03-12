using MediatR;
using ERP.BusinessModels.BaseVM;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class DecryptTokenQuery : IRequest<DecryptTokenModel>
    {
        public string Token { get; set; }
    }
}
