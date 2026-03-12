using MediatR;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.ResponseVM;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.User.Query
{
    public class GetRolesComboQuery:IRequest<List<AspNetRolesModel>>
    {

    }
}
