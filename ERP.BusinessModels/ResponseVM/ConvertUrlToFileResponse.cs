//-----------------------------------------------------------------------
// <copyright file="FileDownloadResponse.cs" company="Possession">
//     Possession copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ResponseVM
{
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// File Download Response class
    /// </summary>
    public class ConvertUrlToFileResponse
    {
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        public IFormFile File { get; set; }
    }
}
