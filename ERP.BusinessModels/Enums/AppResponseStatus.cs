using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BusinessModels.Enums
{
    public enum AppResponseStatus
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
