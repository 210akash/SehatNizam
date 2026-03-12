using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Holiday.Query
{
    public class GetAllHolidayQuery : IRequest<Tuple<IEnumerable<GetHoliday>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}