using MediatR;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.ResponseVM;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Mediator.Mediator.User.Query
{
    public class GetOperatorsComboQuery : IRequest<List<AspNetUsersModel>>
    {
        public string Search { get; set; }
    }
}
