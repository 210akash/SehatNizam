using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.PrimaryOrder.Query
{
    public class GetAllOrderStatusQuery : IRequest<List<GetStatus>>
    {

    }
}