//-----------------------------------------------------------------------
// <copyright file="ResponseInfos.cs" company="Aepistle">
//     copy right Aepistle.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ResponseVM
{
    using ERP.BusinessModels.Enums;

    /// <summary>
    /// Post Tag Response
    /// </summary>
    public class ResponseInfos
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ResponseMessageType Type { get; set; }
    }
}