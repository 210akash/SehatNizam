using System;
namespace ERP.BusinessModels.ResponseVM
{
    public class GetConsultation
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string Subjective { get; set; }
        public string Objective { get; set; }
        public string Assessment { get; set; }
        public string Plan { get; set; }
        public DateTime FollowUpDate { get; set; }
        public long StatusId { get; set; }
        public GetStatus Status { get; set; }
    }
}
