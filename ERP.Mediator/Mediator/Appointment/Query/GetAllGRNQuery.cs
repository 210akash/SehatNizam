using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class GetAllGRNQuery : IRequest<Tuple<IEnumerable<GetGRN>, long>>
    {
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public long? VendorId { get; set; }
        public string Code { get; set; }
        public long StatusId { get; set; }

        public PagingData PagingData { get; set; }
    }
}