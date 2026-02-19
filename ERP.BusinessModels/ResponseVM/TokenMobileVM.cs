//-----------------------------------------------------------------------
// <copyright file="TokenMobile.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ResponseVM
{
    using System;

    /// <summary>
    /// Token Mobile Model
    /// </summary>
    public class TokenMobile
    {
        public Meta Meta { get; set; }
        public Data Data { get; set; }
    }

    /// <summary>
    /// Data Model
    /// </summary>
    public class Data
    {
        public string access_token { get; set; }
    }

    /// <summary>
    /// Meta
    /// </summary>
    public class Meta
    {
        public string Success { get; set; }
        public string Code { get; set; }
    }

}