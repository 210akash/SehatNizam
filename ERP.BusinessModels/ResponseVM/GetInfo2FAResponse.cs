using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BusinessModels.ResponseVM
{
   public class GetInfo2FAResponse
    {
        public bool Enable2FA { get; set; }
        public int? SecurityMethodId { get; set; }
        public string SecurityMethod { get; set; }
    }
}
