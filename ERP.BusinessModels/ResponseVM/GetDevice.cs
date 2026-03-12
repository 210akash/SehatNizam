namespace ERP.BusinessModels.ResponseVM
{
    public class GetDevice
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNo { get; set; }

        public int Port { get; set; }

        public string IPAddress { get; set; }

        public string ConnectionStatus { get; set; }

        public bool IsActive { get; set; }

        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
