using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ERP.BusinessModels.ResponseVM
{
    public class AspnetUserModelResponse
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileBlobUrl { get; set; }
    }
}
