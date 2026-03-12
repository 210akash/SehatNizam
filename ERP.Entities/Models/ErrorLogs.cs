using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public partial class ErrorLogs
    {
        [Key]
        public long Id { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public DateTime Created { get; set; }

        public Guid? UserId { get; set; }

        public string LogLevel { get; set; }

        public string Category { get; set; }
    }

}
