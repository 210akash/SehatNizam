using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Query
{
    public class GetAllShopOrderStatusQuery : IRequest<List<GetStatus>>
    {

    }
}