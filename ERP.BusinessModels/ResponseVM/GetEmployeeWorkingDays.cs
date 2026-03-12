using System;

namespace ERP.Entities.Models
{
    public class GetEmployeeWorkingDays
    {
        public Guid? EmployeeId { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}
