//-----------------------------------------------------------------------
// <copyright file="ResponseInfoExtensions.cs" company="Aepistle">
//     copy right Aepistle.
// </copyright>
//-----------------------------------------------------------------------
namespace ERP.BusinessModels.AttributeExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using ERP.BusinessModels.ResponseVM;

    /// <summary>
    /// Response information Extensions
    /// </summary>
    public static class ResponseInfoExtensions
    {
        /// <summary>
        /// Adds the error.
        /// </summary>
        /// <param name="responseInfos">The response information.</param>
        /// <param name="message">The message.</param>
        public static void AddError(this List<ResponseInfos> responseInfos, string message)
        {
            responseInfos.Add(new ResponseInfos() { Message = message, Type = Enums.ResponseMessageType.Error });
        }

        /// <summary>
        /// Adds the warning.
        /// </summary>
        /// <param name="responseInfos">The response information.</param>
        /// <param name="message">The message.</param>
        public static void AddWarning(this List<ResponseInfos> responseInfos, string message)
        {
            responseInfos.Add(new ResponseInfos() { Message = message, Type = Enums.ResponseMessageType.Warning });
        }

        /// <summary>
        /// Adds the information.
        /// </summary>
        /// <param name="responseInfos">The response information.</param>
        /// <param name="message">The message.</param>
        public static void AddInfo(this List<ResponseInfos> responseInfos, string message)
        {
            responseInfos.Add(new ResponseInfos() { Message = message, Type = Enums.ResponseMessageType.Info });
        }

        /// <summary>
        /// Determines whether this instance has errors.
        /// </summary>
        /// <param name="responseInfos">The response information.</param>
        /// <returns>
        ///   <c>true</c> if the specified response information has errors; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasErrors(this List<ResponseInfos> responseInfos)
        {
            return responseInfos != null && responseInfos.Any(x => x.Type == Enums.ResponseMessageType.Error);
        }

        /// <summary>
        /// Determines whether this instance has warnings.
        /// </summary>
        /// <param name="responseInfos">The response information.</param>
        /// <returns>
        ///   <c>true</c> if the specified response information has warnings; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasWarnings(this List<ResponseInfos> responseInfos)
        {
            return responseInfos != null && responseInfos.Any(x => x.Type == Enums.ResponseMessageType.Warning);
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <param name="responseInfos">The response information.</param>
        /// <returns>The error string</returns>
        public static string GetErrors(this List<ResponseInfos> responseInfos)
        {
            return string.Join(". ", responseInfos.Select(x => x.Message));
        }
    }
}