using ERP.Entities.Models;
using MediatR;
using System;

namespace ERP.Mediator.Mediator.Appointment.Command
{
    public class SaveConsultationCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string Subjective { get; set; }
        public string Objective { get; set; }
        public string Assessment { get; set; }
        public string Plan { get; set; }
        public DateTime FollowUpDate { get; set; }
        public long StatusId { get; set; }
    }
}