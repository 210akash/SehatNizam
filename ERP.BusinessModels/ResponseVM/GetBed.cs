namespace ERP.BusinessModels.ResponseVM
{
    public class GetBed
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string BedNo { get; set; }
        public string Description { get; set; }
        public long RoomId { get; set; }
        public GetRoom Room { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
