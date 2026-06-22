using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Query
{
    public class GetAllAppointmentQuery : IRequest<Tuple<IEnumerable<GetAppointment>, long>>
    {
        public long? Id { get; set; }
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public string PatientName { get; set; }
        public string TokenNo { get; set; }
        public string MRN { get; set; }
        public long? StatusId { get; set; }
        public long? DepartmentId { get; set; }
        public Guid? DoctorId { get; set; }
        public long BookingFormType { get; set; }
        public PagingData PagingData { get; set; }
    }
}