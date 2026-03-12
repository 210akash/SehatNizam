using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.BusinessModels.ParameterVM
{
    /// <summary>
    /// Email model buttons list
    /// </summary>
    public class EmailModelButtons
    {
        /// <summary>
        /// Gets or sets the name of the BTN.
        /// </summary>
        /// <value>
        /// The name of the BTN.
        /// </value>
        public string BtnName { get; set; }

        /// <summary>
        /// Gets or sets the BTN link.
        /// </summary>
        /// <value>
        /// The BTN link.
        /// </value>
        public string BtnLink { get; set; }

        /// <summary>
        /// Gets or sets the color of the BTN background.
        /// </summary>
        /// <value>
        /// The color of the BTN background.
        /// </value>
        public string BtnBgColor { get; set; }
    }
}
