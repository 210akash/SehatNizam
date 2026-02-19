namespace ERP.BusinessModels.Enums
{
    /// <summary>
    /// The Roles Prefix
    /// </summary>
    public enum StatusSensor
    {
        Critical = 1,
        Warning = 2,
        InProgress = 3,
        Stable = 4
    }
    public enum GlobalPlanEnum
    {
        Free = 1,
        Starter = 2,
        Popular = 3,
        Business = 4
    }
    public enum SensorHardwareTypes
    {
        Lora = 1,
        Particle = 2
    }
    public enum NotificationTypes
    {
        AssigneeSetting = 1,
        AssignedSetting = 2,
        MentionedSetting = 3
    }
    public enum SecurityMehtods2FA
    {
        Mobile = 1,
        Email = 2,
        Both = 3
    }
    public enum GatewayType
    {
       Cellular = 1,
        Wifi    = 2
    }
    public enum ReportTypes
    {
        Incident = 1,
        Maintenance = 2
    }
    public enum ResponseMessageType
    {
        /// <summary>
        /// Error Type
        /// </summary>
        Error,

        /// <summary>
        /// Warning Type
        /// </summary>
        Warning,

        /// <summary>
        /// Info Type
        /// </summary>
        Info
    }
}