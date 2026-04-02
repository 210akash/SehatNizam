namespace ERP.BusinessModels.ResponseVM
{
    public class GetPrescription
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string DrugName { get; set; }
        public string Dosage { get; set; }
        public string DrugCode { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }
    }
}
