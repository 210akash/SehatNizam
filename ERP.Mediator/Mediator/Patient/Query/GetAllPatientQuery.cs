using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Patient.Query
{
    public class GetAllPatientQuery : IRequest<Tuple<IEnumerable<GetPatient>, long>>
    {
        public string MRN { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string CNIC { get; set; }
        public long? CityId { get; set; }
        public PagingData PagingData { get; set; }
    }
}