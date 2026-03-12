using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.UOM.Query
{
    public class GetAllUOMQuery : IRequest<Tuple<IEnumerable<GetUOM>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}