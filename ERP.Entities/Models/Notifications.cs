using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Entities.Models
{
    public class Notifications
    {
        [Key]
        public long NotificationId { get; set; }
        public Guid UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Message { get; set; }
    }
}
