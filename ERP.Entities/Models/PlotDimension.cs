using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Entities.Models
{
    public class PlotDimension : BaseEntity
    {
        public long FileId { get; set; }
        public long PossessionsId { get; set; }
        public string DimRight { get; set; }
        public string DimRightBoundry { get; set; }
        public string DimFront { get; set; }
        public string DimFrontBoundry { get; set; }
        public string DimLeft { get; set; }
        public string DimLeftBoundry { get; set; }
        public string DimBack { get; set; }
        [NotMapped]
        public string FileSource { get; set; }
        
        public string DimBackBoundry { get; set; }
        public string DimImg { get; set; }
        public string TotalAreaDim { get; set; }
        public string ExcessLandDim { get; set; }
        public string SizeDim { get; set; }
        public string UnitNoDim { get; set; }
        public string StreetDim { get; set; }
        public string BlockDim { get; set; }
        public string SizeFinalAllot { get; set; }
        public DateTime? CreatedAtDim { get; set; }
        public DateTime? CreatedAtFinalAllot { get; set; }
        public virtual Possessions Possessions { get; set; }

    }
}
