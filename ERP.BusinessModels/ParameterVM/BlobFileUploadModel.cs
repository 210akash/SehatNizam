//-----------------------------------------------------------------------
// <copyright file="BlobFileUploadModel.cs" company="CRM">
//     copy right CRM.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ParameterVM
{
    using System.ComponentModel.DataAnnotations;
    using ERP.BusinessModels.AttributeExtensions;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Blob File Upload model class
    /// </summary>
    public class BlobFileUploadModel
    {
        /// <summary>
        /// Gets or sets the file item.
        /// </summary>
        /// <value>
        /// The file item.
        /// </value>
        [Required(ErrorMessage = "Please submit a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".tif", ".bmp", ".txt", ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".rtf", ".xlsx" })]
        public IFormFile FileItem { get; set; }

        /// <summary>
        /// Gets or sets the name of the folder.
        /// </summary>
        /// <value>
        /// The name of the folder.
        /// </value>
        [Required(ErrorMessage = "Folder Name is required")]
        [StringLength(200, ErrorMessage = "Folder Name must be between 5 and 200 characters", MinimumLength = 5)]
        public string FolderName { get; set; }
    }
}