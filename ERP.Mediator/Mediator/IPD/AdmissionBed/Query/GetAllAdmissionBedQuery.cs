using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionBed.Query
{
    public class GetAllAdmissionBedQuery : IRequest<Tuple<IEnumerable<GetAdmissionBed>, long>>
    {
        public long AdmissionId { get; set; }
        public PagingData PagingData { get; set; }

    }
}