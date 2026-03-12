using System.ComponentModel;

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
        Wifi = 2
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

    public enum OrderStatusEnum
    {
        Create = 1,
        Processed = 2,
        Approved = 3,
        Reject = 4,
        OrderCreate = 10,
        OrderInProcess = 15,
        AccountReviewed = 20,
        OrderConfirm = 30,
        OrderDispatched = 40,
        OrderPartiallyDispatched = 45,
        OrderReceived = 50,
        OrderCanceled = 60,
        OrderReturn = 70,
        DamageClaimInspection = 80,
        DamageClaimDecision = 90,
        OrderDeleted = 100,
        CancelDispatchCreated = 110,
        CancelDispatchForward = 120,
        CancelDispatchSalesReviewed = 130,
        CancelDispatchAccountReviewed = 140,
        CancelDispatchConfirm = 150,
        ManagerApproved = 170,
    }
    public enum AccountsEnum
    {
        CashAccount = 1,
        BankAccount = 2,
        AccountReceivable = 3,
        SaleAccount = 4,
    }

    public enum DeviceType
    {
        Mobile = 1,
        TimeAttendance = 2,
        Manual = 2
    }

    public enum AttendanceType
    {
        [Description("Leave")]
        Leave = 1,
        [Description("Weekly Off")]
        WeeklyOff = 2,
        [Description("Holiday")]
        Holiday = 3,
        [Description("Present")]
        Present = 4,
        [Description("Absent")]
        Absent = 0
    };
}