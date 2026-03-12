using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetHoliday
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
