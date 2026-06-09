using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Admission.Query
{
    public class GetAllAdmissionQuery : IRequest<Tuple<IEnumerable<GetAdmission>, long>>
    {
        public long? Id { get; set; }
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public string PatientName { get; set; }
        public string TokenNo { get; set; }
        public string MRN { get; set; }
        public long? StatusId { get; set; }
        public long? DepartmentId { get; set; }
        public long BookingFormType { get; set; }
        public PagingData PagingData { get; set; }
    }
}