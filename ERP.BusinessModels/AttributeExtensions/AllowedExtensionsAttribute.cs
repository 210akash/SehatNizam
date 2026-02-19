//-----------------------------------------------------------------------
// <copyright file="AllowedExtensionsAttribute.cs" company="CRM">
//     copy right CRM.
// </copyright>
//-----------------------------------------------------------------------
namespace ERP.BusinessModels.AttributeExtensions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Allowed file extensions for upload
    /// </summary>
    /// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        /// <summary>
        /// The extensions
        /// </summary>
        private readonly string[] extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowedExtensionsAttribute"/> class.
        /// </summary>
        /// <param name="extensions">The extensions.</param>
        public AllowedExtensionsAttribute(string[] extensions)
        {
            this.extensions = extensions;
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
            var file = value as IFormFile;
            var extension = Path.GetExtension(file.FileName);
            if (file != null)
            {
                if (!this.extensions.Contains(extension.ToLower()))
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
            return $"This file extension is not allowed!";
        }
    }
}
