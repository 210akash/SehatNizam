using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetBloodRequestLog
    {
        public GetBloodRequest BloodRequest { get; set; }
        public string CurrentStep { get; set; }
        public List<GetBloodRequestLogEntry> Entries { get; set; } = new List<GetBloodRequestLogEntry>();
    }

    public class GetBloodRequestLogEntry
    {
        public DateTime EventDate { get; set; }
        public string Step { get; set; }
        public string Description { get; set; }
        public string Outcome { get; set; }
        public string UnitNo { get; set; }
        public string PerformedBy { get; set; }
        public bool IsReverted { get; set; }
    }
}
