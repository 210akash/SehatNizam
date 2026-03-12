//-----------------------------------------------------------------------
// <copyright file="CustomUserStore.cs" company="sensyrtech">
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
    using Microsoft.Extensions.Configuration;
    using ERP.BusinessModels.BaseVM;
    using ERP.Entities.Models;
    using ERP.Repositories.UnitOfWork;

    /// <summary>
    /// Declaration of Custom User Store class.
    /// </summary>
    public class CustomUserStore :
        IUserLoginStore<AspNetUsersModel>,
        IUserClaimStore<AspNetUsersModel>,
        IUserPasswordStore<AspNetUsersModel>,
        IUserSecurityStampStore<AspNetUsersModel>,
        IUserEmailStore<AspNetUsersModel>,
        IUserLockoutStore<AspNetUsersModel>,
        IUserPhoneNumberStore<AspNetUsersModel>,
        IUserAuthenticationTokenStore<AspNetUsersModel>,
        IUserAuthenticatorKeyStore<AspNetUsersModel>,
        IUserTwoFactorRecoveryCodeStore<AspNetUsersModel>,
        IUserTwoFactorStore<AspNetUsersModel>,
        IQueryableUserStore<AspNetUsersModel>
    {
        #region IQueryableUserStore<AspNetUsersModel>

        /// <summary>
        /// The unit of work
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUserStore"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="configuration">The configuration.</param>
        public CustomUserStore(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets collection of users.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Linq.IQueryable`1" /> collection of users.
        /// </value>
        public IQueryable<AspNetUsersModel> Users
        {
            get { return this.mapper.Map<IQueryable<AspNetUsersModel>>(this.unitOfWork.Repository<AspNetUsers>().Entities).AsQueryable(); }
        }

        #endregion IQueryableUserStore<AspNetUsersModel>

        /// <summary>
        /// Gets Configuration
        /// </summary>
        private IConfiguration Configuration { get; }

        #region IUserLoginStore<AspNetUsersModel>
        /// <summary>
        /// Add Login Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="login">The associated login.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public async Task AddLoginAsync(AspNetUsersModel user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            await this.unitOfWork.Repository<AspNetUserLogins>().AddAsync(this.CreateUserLogin(user, login));
            await this.unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Find By Login Async
        /// </summary>
        /// <param name="loginProvider">the login provider</param>
        /// <param name="providerKey">the provider key</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the Asp Net Users Model</returns>
        public async Task<AspNetUsersModel> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var userLogin = await this.unitOfWork.Repository<AspNetUserLogins>().FindAsync(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
            if (userLogin != null)
            {
                return await this.GetCustomUserAsync(userLogin.UserId);
            }

            return null;
        }

        /// <summary>
        /// Get Logins Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the user login info</returns>
        public async Task<IList<UserLoginInfo>> GetLoginsAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            var vas = await this.unitOfWork.Repository<AspNetUserLogins>().FindAllAsync(x => x.UserId == user.Id);

            return vas.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName)).ToList();
        }

        /// <summary>
        /// Remove Login Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="loginProvider">the login provider</param>
        /// <param name="providerKey">the provider key</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public async Task RemoveLoginAsync(AspNetUsersModel user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userLogin = await this.unitOfWork.Repository<AspNetUserLogins>().FindAsync(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey && x.UserId == user.Id);

            this.unitOfWork.Repository<AspNetUserLogins>().Remove(userLogin);
            await this.unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Create the user Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the identity result</returns>
        public async Task<IdentityResult> CreateAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userData = this.mapper.Map<AspNetUsers>(user);
            userData.CreatedDate = DateTime.UtcNow;
            userData.LockoutEnabled = false;
            userData.LockoutEnd = null;
            await this.unitOfWork.Repository<AspNetUsers>().AddAsync(userData);

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
        /// Delete the User Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the identity result</returns>
        public async Task<IdentityResult> DeleteAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var users = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(x => x.Id == user.Id);
            this.unitOfWork.Repository<AspNetUsers>().Remove(users);

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
        /// Find By Id Async
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the asp net users model</returns>
        public async Task<AspNetUsersModel> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await this.GetCustomUserAsync(Guid.Parse(userId));
        }

        /// <summary>
        /// Find By Name Async
        /// </summary>
        /// <param name="normalizedUserName">the normalize user name</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the asp net users model</returns>
        public async Task<AspNetUsersModel> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(x => x.Email == normalizedUserName);
            return this.mapper.Map<AspNetUsersModel>(user);
        }

        /// <summary>
        /// Gets the normalized user name for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the normalized user name for the specified <paramref name="user" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">The user null exception</exception>
        public Task<string> GetNormalizedUserNameAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Gets the user identifier for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose identifier should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">The user null exception</exception>
        public Task<string> GetUserIdAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id.ToString());
        }

        /// <summary>
        /// Get User Name Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the user name</returns>
        public Task<string> GetUserNameAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Set Normalized User Name Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="normalizedName">the normalized Name</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the user name</returns>
        public Task SetNormalizedUserNameAsync(AspNetUsersModel user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = normalizedName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set User Name Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="userName">the user Name</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the user name</returns>
        public Task SetUserNameAsync(AspNetUsersModel user, string userName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = userName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Update the user Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the identity result</returns>
        public async Task<IdentityResult> UpdateAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var users = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(x => x.Id == user.Id);
            users.AccessFailedCount = user.AccessFailedCount;
            users.ConcurrencyStamp = user.ConcurrencyStamp;
            users.Email = user.Email;
            users.EmailConfirmed = user.EmailConfirmed;
            users.FirstName = user.FirstName;
            users.LastName = user.LastName;
            users.LockoutEnabled = user.LockoutEnabled;
            users.LockoutEnd = user.LockoutEnd;
            users.NormalizedEmail = user.NormalizedEmail;
            users.NormalizedUserName = user.NormalizedUserName;
            users.PasswordHash = user.PasswordHash;
            users.PhoneNumber = user.PhoneNumber;
            users.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            users.SecurityStamp = user.SecurityStamp;
            users.TwoFactorEnabled = user.TwoFactorEnabled;
            users.UserName = user.UserName;
            this.unitOfWork.Repository<AspNetUsers>().Update(users);

            try
            {
                await this.unitOfWork.SaveChangesAsync();
                this.unitOfWork.Repository<AspNetUsers>().DetachEntry(users);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// dispose the entry
        /// </summary>
        public void Dispose()
        {
        }
        #endregion IUserLoginStore<AspNetUsersModel>

        #region IUserClaimStore<AspNetUsersModel>
        /// <summary>
        /// Add Claims Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="claims">the claims</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public async Task AddClaimsAsync(AspNetUsersModel user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                await this.unitOfWork.Repository<AspNetUserClaims>().AddAsync(this.CreateUserClaim(user, claim));
                await this.unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Get Claims Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the claims list</returns>
        public async Task<IList<Claim>> GetClaimsAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            var userClaims = await this.unitOfWork.Repository<AspNetUserClaims>().FindAllAsync(x => x.UserId == user.Id);

            return userClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        /// <summary>
        /// Returns a list of users who contain the specified <see cref="T:System.Security.Claims.Claim" />.
        /// </summary>
        /// <param name="claim">The claim to look for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the result of the asynchronous query, a list of <typeparamref name="TUser" /> who
        /// contain the specified claim.
        /// </returns>
        /// <exception cref="ArgumentNullException">The claim null exception</exception>
        public async Task<IList<AspNetUsersModel>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var matchedClaims = await this.unitOfWork.Repository<AspNetUserClaims>().FindAllAsync(x => x.ClaimType == claim.Type
                                                                                                && x.ClaimValue == claim.Value);
            var vm_Customer = new List<AspNetUsers>();

            foreach (var item in matchedClaims)
            {
                var vm_user = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(x => x.Id == item.UserId);
                vm_Customer.Add(vm_user);
            }

            return this.mapper.Map<List<AspNetUsersModel>>(vm_Customer);
        }

        /// <summary>
        /// Removes the specified <paramref name="claims" /> from the given <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user to remove the specified <paramref name="claims" /> from.</param>
        /// <param name="claims">A collection of <see cref="T:System.Security.Claims.Claim" />s to remove.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <exception cref="ArgumentNullException">
        /// user
        /// or
        /// claims
        /// </exception>
        /// <returns>the task</returns>
        public async Task RemoveClaimsAsync(AspNetUsersModel user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                var matchedClaims = await this.unitOfWork.Repository<AspNetUserClaims>().FindAllAsync(x => x.ClaimType == claim.Type
                && x.ClaimValue == claim.Value && x.UserId == user.Id);
                foreach (var c in matchedClaims)
                {
                    this.unitOfWork.Repository<AspNetUserClaims>().Remove(c);
                    await this.unitOfWork.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Replaces the given <paramref name="claim" /> on the specified <paramref name="user" /> with the <paramref name="newClaim" />
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim to replace.</param>
        /// <param name="newClaim">The new claim to replace the existing <paramref name="claim" /> with.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <exception cref="ArgumentNullException">
        /// user
        /// or
        /// claim
        /// or
        /// newClaim
        /// </exception>
        /// <returns>the task</returns>
        public async Task ReplaceClaimAsync(AspNetUsersModel user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            var matchedClaims = await this.unitOfWork.Repository<AspNetUserClaims>().FindAllAsync(x => x.ClaimType == claim.Type
            && x.ClaimValue == claim.Value && x.UserId == user.Id);
            foreach (var matchedClaim in matchedClaims)
            {
                matchedClaim.ClaimValue = newClaim.Value;
                matchedClaim.ClaimType = newClaim.Type;
            }
        }
        #endregion IUserClaimStore<AspNetUsersModel>

        #region IUserEmailStore<AspNetUsersModel>
        /// <summary>
        /// Find By Email Async
        /// </summary>
        /// <param name="normalizedEmail">the normalize email</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the asp net user model</returns>
        public async Task<AspNetUsersModel> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var user = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(x => x.Email == normalizedEmail);
            return this.mapper.Map<AspNetUsersModel>(user);
        }

        /// <summary>
        /// Get Email Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the email address</returns>
        public Task<string> GetEmailAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Get Email Confirmed Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>return true or false</returns>
        public Task<bool> GetEmailConfirmedAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Get Normalized Email Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the normalize email</returns>
        public Task<string> GetNormalizedEmailAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Set Email Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="email">the email</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetEmailAsync(AspNetUsersModel user, string email, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = email;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set Email Confirmed Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="confirmed">is confirmed</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetEmailConfirmedAsync(AspNetUsersModel user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set Normalized Email Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="normalizedEmail">the normalize email</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetNormalizedEmailAsync(AspNetUsersModel user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = normalizedEmail;
            return Task.CompletedTask;
        }
        #endregion IUserEmailStore<AspNetUsersModel>

        #region IUserAuthenticationTokenStore<AspNetUsersModel>
        /// <summary>
        /// Get Token Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="loginProvider">the login provider</param>
        /// <param name="name">the name</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the token</returns>
        public async Task<string> GetTokenAsync(AspNetUsersModel user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var entry = await this.unitOfWork.Repository<AspNetUserTokens>().FindAsync(x => x.LoginProvider == loginProvider
            && x.Name == name && x.UserId == user.Id);
            return entry?.Value;
        }

        /// <summary>
        /// Remove Token Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="loginProvider">the login provider</param>
        /// <param name="name">the name</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public async Task RemoveTokenAsync(AspNetUsersModel user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var entry = await this.unitOfWork.Repository<AspNetUserTokens>().FindAsync(x => x.LoginProvider == loginProvider
            && x.Name == name && x.UserId == user.Id);

            if (entry != null)
            {
                this.unitOfWork.Repository<AspNetUserTokens>().Remove(entry);
                await this.unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Sets the token value for a particular user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="value">The value of the token.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <exception cref="ArgumentNullException">the user null exception</exception>
        /// <returns>the task</returns>
        public async Task SetTokenAsync(AspNetUsersModel user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var entry = await this.unitOfWork.Repository<AspNetUserTokens>().FindAsync(x => x.LoginProvider == loginProvider
            && x.Name == name && x.UserId == user.Id);

            if (entry == null)
            {
                await this.unitOfWork.Repository<AspNetUserTokens>().AddAsync(this.CreateUserToken(user, loginProvider, name, value));
                await this.unitOfWork.SaveChangesAsync();
            }
            else
            {
                entry.Value = value;
            }
        }
        #endregion IUserAuthenticationTokenStore<AspNetUsersModel>

        #region IUserTwoFactorRecoveryCodeStore<AspNetUsersModel>
        /// <summary>
        /// Count Codes Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the count</returns>
        public async Task<int> CountCodesAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var mergedCodes = await this.GetTokenAsync(user, this.Configuration["JwtSecurityToken:Issuer"], this.Configuration["JwtSecurityToken:Audience"], cancellationToken) ?? string.Empty;
            if (mergedCodes.Length > 0)
            {
                return mergedCodes.Split(';').Length;
            }

            return 0;
        }

        /// <summary>
        /// Redeem Code Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="code">the code</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>is redeemed code</returns>
        public async Task<bool> RedeemCodeAsync(AspNetUsersModel user, string code, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var mergedCodes = await this.GetTokenAsync(user, this.Configuration["JwtSecurityToken:Issuer"], this.Configuration["JwtSecurityToken:Audience"], cancellationToken) ?? string.Empty;
            var splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code))
            {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await this.ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Replace Codes Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="recoveryCodes">the recovery codes</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public async Task ReplaceCodesAsync(AspNetUsersModel user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            var mergedCodes = string.Join(";", recoveryCodes);
            await this.SetTokenAsync(user, this.Configuration["JwtSecurityToken:Issuer"], this.Configuration["JwtSecurityToken:Audience"], mergedCodes, cancellationToken);
        }
        #endregion IUserTwoFactorRecoveryCodeStore<AspNetUsersModel>

        #region IUserTwoFactorRecoveryCodeStore<AspNetUsersModel>
        /// <summary>
        /// Get Authenticator Key Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the authenticator key</returns>
        public async Task<string> GetAuthenticatorKeyAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            return await this.GetTokenAsync(user, this.Configuration["JwtSecurityToken:Issuer"], this.Configuration["JwtSecurityToken:Audience"], cancellationToken);
        }

        /// <summary>
        /// Set Authenticator Key Async
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="key">the key</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public async Task SetAuthenticatorKeyAsync(AspNetUsersModel user, string key, CancellationToken cancellationToken)
        {
            await this.SetTokenAsync(user, this.Configuration["JwtSecurityToken:Issuer"], this.Configuration["JwtSecurityToken:Audience"], key, cancellationToken);
        }
        #endregion IUserTwoFactorRecoveryCodeStore<AspNetUsersModel>

        #region IUserPasswordStore<AspNetUsersModel>

        /// <summary>
        /// Get Password Hash Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the password hash</returns>
        public Task<string> GetPasswordHashAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        /// Has Password Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>has password hash</returns>
        public Task<bool> HasPasswordAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        /// <summary>
        /// Set Password Hash Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="passwordHash">the password hash</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetPasswordHashAsync(AspNetUsersModel user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        #endregion IUserPasswordStore<AspNetUsersModel>

        #region IUserSecurityStampStore<AspNetUsersModel>

        /// <summary>
        /// Get Security Stamp Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the security stamp</returns>
        public Task<string> GetSecurityStampAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        /// Set Security Stamp Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="stamp">the security stamp</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetSecurityStampAsync(AspNetUsersModel user, string stamp, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (stamp == null)
            {
                throw new ArgumentNullException(nameof(stamp));
            }

            if (string.IsNullOrEmpty(user.SecurityStamp))
            {
                user.SecurityStamp = stamp;
            }

            return Task.CompletedTask;
        }
        #endregion IUserSecurityStampStore<AspNetUsersModel>

        #region IUserLockoutStore<AspNetUsersModel>

        /// <summary>
        /// Get Access Failed Count Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task<int> GetAccessFailedCountAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Get Lockout Enabled Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>is Enabled</returns>
        public Task<bool> GetLockoutEnabledAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        /// Get Lockout End Date Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the lockout end date</returns>
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            DateTimeOffset? localTime2 = user.LockoutEnd != null ? user.LockoutEnd
                : DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            return Task.FromResult(localTime2);
        }

        /// <summary>
        /// Increment Access Failed Count Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the Increment Access Failed Count</returns>
        public Task<int> IncrementAccessFailedCountAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Reset Access Failed Count Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task ResetAccessFailedCountAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set Lockout Enabled Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="enabled">is enabled</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetLockoutEnabledAsync(AspNetUsersModel user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set Lockout End Date Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="lockoutEnd">the lockout end</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetLockoutEndDateAsync(AspNetUsersModel user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnd = lockoutEnd != null ? lockoutEnd.Value.DateTime
                : DateTime.UtcNow;

            return Task.CompletedTask;
        }

        #endregion IUserLockoutStore<AspNetUsersModel>

        #region IUserPhoneNumberStore<AspNetUsersModel>

        /// <summary>
        /// Get Phone Number Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the phone number</returns>
        public Task<string> GetPhoneNumberAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Get Phone Number Confirmed Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the phone number confirmed</returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Set Phone Number Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="phoneNumber">the phone number</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetPhoneNumberAsync(AspNetUsersModel user, string phoneNumber, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set Phone Number Confirmed Async
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="confirmed">is confirmed</param>
        /// <param name="cancellationToken">CancellationToken model</param>
        /// <returns>the task</returns>
        public Task SetPhoneNumberConfirmedAsync(AspNetUsersModel user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }
        #endregion IUserPhoneNumberStore<AspNetUsersModel>

        #region IUserTwoFactorStore<AspNetUsersModel>

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a flag indicating whether the specified
        /// <paramref name="user" /> has two factor authentication enabled or not.
        /// </returns>
        /// <exception cref="ArgumentNullException">the user null exception</exception>
        public Task<bool> GetTwoFactorEnabledAsync(AspNetUsersModel user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }

        /// <summary>
        /// Sets a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="enabled">A flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">the user null exception</exception>
        public Task SetTwoFactorEnabledAsync(AspNetUsersModel user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        #endregion IUserTwoFactorStore<AspNetUsersModel>

        #region Private Methods

        /// <summary>
        /// Called to create a new instance of a <see cref="IdentityUserLogin{TKey}"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="login">The associated login.</param>
        /// <returns>the Asp Net User Logins</returns>
        private AspNetUserLogins CreateUserLogin(AspNetUsersModel user, UserLoginInfo login)
        {
            return new AspNetUserLogins
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        /// <summary>
        /// Called to create a new instance of a <see cref="IdentityUserToken{TKey}"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="loginProvider">The associated login provider.</param>
        /// <param name="name">The name of the user token.</param>
        /// <param name="value">The value of the user token.</param>
        /// <returns>the Asp Net User Tokens</returns>
        private AspNetUserTokens CreateUserToken(AspNetUsersModel user, string loginProvider, string name, string value)
        {
            return new AspNetUserTokens
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }

        /// <summary>
        /// Create User Claim
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>the Asp Net User Claims</returns>
        private AspNetUserClaims CreateUserClaim(AspNetUsersModel user, Claim claim)
        {
            var userClaim = new AspNetUserClaims
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
            };

            return userClaim;
        }

        /// <summary>
        /// Gets the custom user asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The asp net user model</returns>
        private async Task<AspNetUsersModel> GetCustomUserAsync(Guid userId)
        {
            var user = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(x => x.Id == userId);
            return this.mapper.Map<AspNetUsersModel>(user);
        }
        #endregion Private Methods
    }
}
