using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.SugarType.Query
{
    public class GetAllSugarTypeQuery : IRequest<Tuple<IEnumerable<GetSugarType>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}