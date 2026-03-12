using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetRequest
    {
        public long Id { get; set; }
        public string RegNo { get; set; }
        public string ReqNo { get; set; }
        public long FileId { get; set; }
        public string BlockName { get; set; }
        public string Status { get; set; }
        public long MemberId { get; set; }
        public string Member { get; set; }
        public string BuyerName { get; set; }
        public string BuyerCNIC { get; set; }
        public string BuyerNumber { get; set; }
        public string VisitorName { get; set; }
        public string VisitorCNIC { get; set; }
        public string VisitorNumber { get; set; }
        public string Comments { get; set; }
    }
}
