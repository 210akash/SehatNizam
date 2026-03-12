namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeBank
    {
        public long Id { get; set; }
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
