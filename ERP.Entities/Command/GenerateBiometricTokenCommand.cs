
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Entities.Models;

namespace ERP.Entities.Command
{
    public class GenerateBiometricTokenCommand 
    {
        public long? memberId { get; set; }
        public string metaInfoTitle { get; set; }
        public long metaId { get; set; }
        public long metaIntimationId { get; set; }
        public Guid createdById { get; set; }
    }
}
