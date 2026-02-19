namespace ERP.BusinessModels.ResponseVM
{
    public class GetCompany
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string NTN { get; set; }
        public string Phone { get; set; }
        public string Color { get; set; }
        public GetUser CreatedBy { get; set; }
        public string Logo { get; set; }
    }
}
