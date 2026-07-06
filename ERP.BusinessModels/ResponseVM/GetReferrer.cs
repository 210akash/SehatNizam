namespace ERP.BusinessModels.ResponseVM
{
    public class GetReferrer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Hospital { get; set; }
        public string PhoneNo { get; set; }
        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }

        public virtual GetAccount Account { get; set; }
        public long? AccountId { get; set; }

        public virtual GetAccountGroup AccountGroup { get; set; }
        public long? AccountGroupId { get; set; }

        public GetUser CreatedBy { get; set; }
    }
}
