//-----------------------------------------------------------------------
// <copyright file="ResponseStatus.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.Enums
{
    /// <summary>
    /// Response Status
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary>
        /// Success Type
        /// </summary>
        OK = 200,

        /// <summary>
        /// Error Type
        /// </summary>
        Error = 500,

        /// <summary>
        /// Info Type
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warning Type
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Limit Exceeded Type
        /// </summary>
        LimitExceeded = 4,


        InvalidToken = 402,

        /// <summary>
        /// Forbidden Type
        /// </summary>
        Forbidden = 403,
        BadRequest = 404,
        NoUserExists = 405,
        /// <summary>
        /// Unauthorized Type
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// No Content Type
        /// </summary>
        NoContent = 204,

        /// <summary>
        /// No Conflict Type
        /// </summary>
        Conflict = 409,

        DateNotMatch = 100,
        InvalidDSF = 101,
        InvalidShiftTime = 102,
        NoRouteFound = 103,
        NoVisitsToday = 104,
        MACAddressNotMatched = 105,
        ValidationFailed = 410,
        RecordNotFound = 411,
        DuplicatePhoneNo = 412,
        DuplicateRecord = 413,
        WeeklyOff = 414,
    }
}
