namespace ERP.BusinessModels.ResponseVM
{
    public class GetPatientProblem
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string Problem { get; set; }
        public string Onset { get; set; }
        public GetStatus Status { get; set; }
        public long StatusId { get; set; }
    }
}
