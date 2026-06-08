using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class AdmissionRoundMedication : BaseEntity
    {
        public long AdmissionRoundId { get; set; }
        public Guid? ConsultantsAssistantId { get; set; }
        public long ItemGroupId { get; set; }
        public string Dose { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string Route { get; set; }
        public string Instructions { get; set; }
        public AdmissionRound AdmissionRound { get; set; }
        public AspNetUsers ConsultantsAssistant { get; set; }
        public ItemGroup ItemGroup { get; set; }
        public ICollection<MedicationAdministration> MedicationAdministrations { get; set; }
    }
}
