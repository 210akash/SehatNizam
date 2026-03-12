namespace ERP.BusinessModels.ResponseVM
{
    using System;
    using System.Collections.Generic;

    public class ConfirmRegisterEmailResponse
    {
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int? IndustryId { get; set; }
        public string UserEmail { get; set; }
    }
}