//-----------------------------------------------------------------------
// <copyright file="FileDownloadResponse.cs" company="Possession">
//     Possession copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ResponseVM
{
    using System.IO;

    /// <summary>
    /// File Download Response class
    /// </summary>
    public class FileDownloadResponse
    {
        /// <summary>
        /// Gets or sets the file stream.
        /// </summary>
        /// <value>
        /// The file stream.
        /// </value>
        public Stream FileStream { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the type of the file content.
        /// </summary>
        /// <value>
        /// The type of the file content.
        /// </value>
        public string FileContentType { get; set; }
    }
}
