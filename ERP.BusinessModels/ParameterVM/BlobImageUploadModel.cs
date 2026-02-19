//-----------------------------------------------------------------------
// <copyright file="BlobImageUploadModel.cs" company="CRM">
//     copy right CRM.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ParameterVM
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Blob Image Upload model class
    /// </summary>
    public class BlobImageUploadModel
    {
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [Required(ErrorMessage = "File is required")]
        [MinLength(10, ErrorMessage = "File must has minimum 10 characters")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        [Required(ErrorMessage = "File Name is required")]
        [StringLength(200, ErrorMessage = "File Name must be between 5 and 200 characters", MinimumLength = 5)]
        public string FileName { get; set; }

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