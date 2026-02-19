using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Entities.Models
{
    /// <summary>
    /// BaseApiModel Setting Implementation 
    /// </summary>
    public class BaseApi
    {
        /// <summary>
        /// Gets or sets the Meta.
        /// </summary>
        /// <value>
        /// The Meta.
        /// </value>
        public Meta Meta { get; set; }
    }

    public class Meta
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        /// <value>
        /// The Id.
        /// </value>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Code.
        /// </summary>
        /// <value>
        /// The Code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Data.
        /// </summary>
        /// <value>
        /// The send Data.
        /// </value>
        public object Data { get; set; }
    }
}
