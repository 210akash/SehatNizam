using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.ComplexTypes;
using ERP.Mediator.Mediator.Auth.Query;
//using ERP.Mediator.Mediator.ERP.Query;
using ERP.Repositories.UnitOfWork;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class GetAllUsersByRoleHandler : IRequestHandler<GetAllUsersByRoleQuery, List<GetUsers>>
    {
        /// <summary>
        /// Unit of work Declaration
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Mapper Declaration
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllTemplatesHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public GetAllUsersByRoleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<List<GetUsers>> Handle(GetAllUsersByRoleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //var roleId = unitOfWork.Repository<global::ERP.Entities.Models.AspNetRoles>().GetAllAsync(null, null, "AspNetUserRoles").Result.Where(x => x.Name.ToString().ToLower() == request.role).FirstOrDefault().Id;
                var users = unitOfWork.Repository<global::ERP.Entities.Models.AspNetUsers>().GetAsync(x => x.IsActive == true, null, null, "AspNetUserRoles").Result.Where(x => x.AspNetUserRoles.Any(x => x.RoleId == new Guid(request.role)));
                var _user = mapper.Map<List<GetUsers>>(users);
                //foreach (var item in _user)
                //{
                //    List<string> roleids = new List<string>();
                //    foreach (var role in item.AspNetUserRoles.ToList())
                //    {
                //        roleids.Add(role.RoleId.ToString());
                //    }
                //    item.RoleId = roleids.ToArray();
                //    item.AspNetUserRoles = null;
                //}

                return _user;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


    }
}

