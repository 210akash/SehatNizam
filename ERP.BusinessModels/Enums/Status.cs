//-----------------------------------------------------------------------
// <copyright file="Status.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace CRM.BusinessModels.Enums
{
    /// <summary>
    /// Response Status
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Lead Created
        /// </summary>
        LeadCreated = 1,

        /// <summary>
        /// Lead InProcess
        /// </summary>
        LeadInProcess = 5,

        /// <summary>
        /// Lead Closed
        /// </summary>
        LeadClosed = 10,

        /// <summary>
        /// Lead Done
        /// </summary>
        LeadDone = 15,
    }
}
