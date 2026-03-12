using ERP.BusinessModels.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.BusinessModels.ParameterVM
{
    /// <summary>
    /// Email model class
    /// </summary>
    public class SMSCallModel
    {
        public string MachineStateName { get; set; }
        public string SensorName { get; set; }
        public string SensorStatusName { get; set; }
        public decimal? LiveValue { get; set; }
        public string UnitName { get; set; }
        public string MachineName { get; set; }
        public string Color { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateTime { get; set; }
        public string Email { get; set; }
        public string NotifyMessage { get; set; }
    }
}
