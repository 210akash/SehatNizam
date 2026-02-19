using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Entities.Models
{
    public partial class Logs
    {
        [Key]
        public long Id { get; set; }

        public string LogLevel { get; set; }

        public string Message { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Query { get; set; }

        public DateTime Created { get; set; }

        public Guid? UserId { get; set; }

        public virtual AspNetUsers User { get; set; }

    }
}
