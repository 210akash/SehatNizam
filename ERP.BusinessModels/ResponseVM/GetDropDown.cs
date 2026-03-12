using Microsoft.VisualBasic;
using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetDropDown
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
