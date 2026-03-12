using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.City.Query
{
    public class GetAllCityQuery : IRequest<Tuple<IEnumerable<GetCity>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}