using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.IGP.Query
{
    public class GetAllIGPQuery : IRequest<Tuple<IEnumerable<GetIGP>, long>>
    {
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        //public long? CompanyId { get; set; }
        public string Code { get; set; }
        public long StatusId { get; set; }

        public PagingData PagingData { get; set; }
    }
}