//-----------------------------------------------------------------------
// <copyright file="CustomRoleStore.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Core.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using global::AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using ERP.BusinessModels.BaseVM;
    using ERP.Entities.Models;
    using ERP.Repositories.UnitOfWork;

    /// <summary>
    /// Declaration of Custom Role Store class.
    /// </summary>
    public class CustomRoleStore : IRoleStore<AspNetRolesModel>, IQueryableRoleStore<AspNetRolesModel>, IRoleClaimStore<AspNetRolesModel>
    {
        /// <summary>
        /// The unit of work
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRoleStore"/> class
        /// </summary>
        /// <param name="unitOfWork">the unit of work instance</param>
        /// <param name="mapper">the auto mapper instance</param>
        public CustomRoleStore(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        #region IQueryableRoleStore<AspNetRolesModel>

        /// <summary>
        /// Gets collection of roles.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Linq.IQueryable`1" /> collection of roles.
        /// </value>
        public IQueryable<AspNetRolesModel> Roles
        {
            get { return this.mapper.Map<IQueryable<AspNetRolesModel>>(this.unitOfWork.Repository<AspNetRoles>().Entities).AsQueryable(); }
        }
        #endregion IQueryableRoleStore<AspNetRolesModel>

        #region IQueryableRoleStore<AspNetRolesModel>

        /// <summary>
        /// create the roles
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the Identity Result</returns>
        public async Task<IdentityResult> CreateAsync(AspNetRolesModel role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var roleData = this.mapper.Map<AspNetRoles>(role);
            roleData.Id = Guid.NewGuid();
            await this.unitOfWork.Repository<AspNetRoles>().AddAsync(roleData);

            try
            {
                await this.unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// delete the roles
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the Identity Result</returns>
        public async Task<IdentityResult> DeleteAsync(AspNetRolesModel role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            this.unitOfWork.Repository<AspNetRoles>().Remove(this.mapper.Map<AspNetRoles>(role));

            try
            {
                await this.unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// dispose the instance
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Find by role id
        /// </summary>
        /// <param name="roleId">role Id</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the Asp Net Roles Model</returns>
        public async Task<AspNetRolesModel> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var role = await this.unitOfWork.Repository<AspNetRoles>().FindAsync(x => x.Id == Guid.Parse(roleId));
            return this.mapper.Map<AspNetRolesModel>(role);
        }

        /// <summary>
        /// Find by normalized role name
        /// </summary>
        /// <param name="normalizedRoleName">normalized role name</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the Asp Net Roles Model</returns>
        public async Task<AspNetRolesModel> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var role = await this.unitOfWork.Repository<AspNetRoles>().FindAsync(x => x.Name == normalizedRoleName);
            return this.mapper.Map<AspNetRolesModel>(role);
        }

        /// <summary>
        /// Get Normalized Role Name Async
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the normalized role name</returns>
        public Task<string> GetNormalizedRoleNameAsync(AspNetRolesModel role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Name);
        }

        /// <summary>
        /// Get Normalized Role Id Async
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the Role Id</returns>
        public Task<string> GetRoleIdAsync(AspNetRolesModel role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Id.ToString());
        }

        /// <summary>
        /// Get Role Name Async
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the Role Name</returns>
        public Task<string> GetRoleNameAsync(AspNetRolesModel role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Name);
        }

        /// <summary>
        /// Set Normalized Role Name Async
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="normalizedName">normalized name</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetNormalizedRoleNameAsync(AspNetRolesModel role, string normalizedName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.Name = normalizedName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set Role Name Async
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="roleName">the role name</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetRoleNameAsync(AspNetRolesModel role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.Name = roleName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Update the role Async
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the Identity Result</returns>
        public async Task<IdentityResult> UpdateAsync(AspNetRolesModel role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var roleModel = await this.unitOfWork.Repository<AspNetRoles>().FindAsync(x => x.Id == role.Id);
            roleModel.ConcurrencyStamp = role.ConcurrencyStamp;
            roleModel.Name = role.Name;
            roleModel.NormalizedName = role.NormalizedName;

            this.unitOfWork.Repository<AspNetRoles>().Update(roleModel);

            try
            {
                await this.unitOfWork.SaveChangesAsync();
                this.unitOfWork.Repository<AspNetRoles>().DetachEntry(roleModel);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;
        }

        #endregion IRoleStore<AspNetRolesModel>

        #region IRoleClaimStore<AspNetRolesModel>

        /// <summary>
        /// Add Claim Async
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="claim">the claims model</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public async Task AddClaimAsync(AspNetRolesModel role, Claim claim, CancellationToken cancellationToken = default)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var model = new AspNetUserClaims()
            {
                Id = Guid.NewGuid(),
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                UserId = role.Id
            };

            await this.unitOfWork.Repository<AspNetUserClaims>().AddAsync(model);
            await this.unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Get Claims Async
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the claims list</returns>
        public async Task<IList<Claim>> GetClaimsAsync(AspNetRolesModel role, CancellationToken cancellationToken = default)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var rolesClaim = await this.unitOfWork.Repository<AspNetRoleClaims>().FindAllAsync(x => x.RoleId == role.Id);
            return rolesClaim.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        /// <summary>
        /// Remove Claim Async
        /// </summary>
        /// <param name="role">AspNetRolesModel model</param>
        /// <param name="claim">the claims model</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public async Task RemoveClaimAsync(AspNetRolesModel role, Claim claim, CancellationToken cancellationToken = default)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var rolesClaim = await this.unitOfWork.Repository<AspNetRoleClaims>().FindAllAsync(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value && x.RoleId == role.Id);

            foreach (var c in rolesClaim)
            {
                this.unitOfWork.Repository<AspNetRoleClaims>().Remove(c);
                await this.unitOfWork.SaveChangesAsync();
            }
        }

        #endregion IRoleClaimStore<AspNetRolesModel>
    }
}
