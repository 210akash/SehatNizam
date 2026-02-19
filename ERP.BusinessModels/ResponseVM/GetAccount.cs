namespace ERP.BusinessModels.ResponseVM
{
    public class GetAccount
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Opening { get; set; }
        public long AccountTypeId { get; set; }
        public GetAccountType AccountType { get; set; }

        public long AccountFlowId { get; set; }
        public GetAccountFlow AccountFlow { get; set; }
        
        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
