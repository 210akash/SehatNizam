//-----------------------------------------------------------------------
// <copyright file="AutoMapperConfiguration.cs" company="Aepistle">
//     copy right Aepistle.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Core.AutoMapper.Configuration
{
    using System.Linq;
    using global::AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using ERP.BusinessModels.BaseVM;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Entities.Models;

    /// <summary>
    /// Auto Mapper Configuration
    /// </summary>
    public partial class AutoMapperConfiguration : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperConfiguration"/> class.
        /// </summary>
        public AutoMapperConfiguration()
        {
            this.CreateMap<AspNetUsers, AspNetUsersModel>();
            this.CreateMap<AspNetUsers, AspNetUsersModel>().ReverseMap();
            this.CreateMap<AspNetRoles, AspNetRolesModel>();
            this.CreateMap<AspNetRoles, AspNetRolesModel>().ReverseMap();
            this.CreateMap<IdentityResult, IdentityResponse>();
        }
    }
}
