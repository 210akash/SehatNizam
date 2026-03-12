//-----------------------------------------------------------------------
// <copyright file="AspNetRolesModel.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.BaseVM
{
    /// <summary>
    /// Declaration of Asp Net Roles Model class.
    /// </summary>
    public class AspNetRolesModel : BaseEntityModel
    {
        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Normalized Name
        /// </summary>
        public string NormalizedName { get; set; }

        /// <summary>
        /// Gets or sets the Concurrency Stamp
        /// </summary>
        public string ConcurrencyStamp { get; set; }
    }
}