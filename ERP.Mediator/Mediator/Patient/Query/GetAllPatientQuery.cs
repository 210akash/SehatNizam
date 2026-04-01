using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Patient.Query
{
    public class GetAllPatientQuery : IRequest<Tuple<IEnumerable<GetPatient>, long>>
    {
        public string Name { get; set; }
        public long? ProjectId { get; set; }
        public long? CityId { get; set; }
        public PagingData PagingData { get; set; }
    }
}