//-----------------------------------------------------------------------
// <copyright file="MaxFileSizeAttribute.cs" company="CRM">
//     copy right CRM.
// </copyright>
//-----------------------------------------------------------------------
namespace ERP.BusinessModels.AttributeExtensions
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Maximum file size allowed to upload
    /// </summary>
    /// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        /// <summary>
        /// The maximum file size
        /// </summary>
        private readonly int maxFileSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxFileSizeAttribute"/> class.
        /// </summary>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public MaxFileSizeAttribute(int maxFileSize)
        {
            this.maxFileSize = maxFileSize;
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.
        /// </returns>
        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            //// var extension = Path.GetExtension(file.FileName);
            //// var allowedExtensions = new[] { ".jpg", ".png" };`enter code here`
            if (value is IFormFile file)
            {
                if (file.Length > this.maxFileSize)
                {
                    return new ValidationResult(this.GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <returns>Error Message</returns>
        private string GetErrorMessage()
        {
            return $"Maximum allowed file size is { this.maxFileSize} bytes.";
        }
    }
}
