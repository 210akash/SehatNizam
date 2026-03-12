using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Auth.Query;
using ERP.Repositories.UnitOfWork;
using System.Linq.Expressions;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class GetAllSaleUsersHandler : IRequestHandler<GetAllSaleUsersQuery, Tuple<List<GetAllUsers>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetAllSaleUsersHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<List<GetAllUsers>, long>> Handle(GetAllSaleUsersQuery request, CancellationToken cancellationToken)
        {
            var department = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name.ToLower() == "sale");
            Expression<Func<Entities.Models.AspNetUsers, bool>> predicate = x => x.DepartmentId == department.Id &&
                (string.IsNullOrEmpty(request.Name)
                || x.FirstName.ToLower().Contains(request.Name.ToLower())
                || x.LastName.ToLower().Contains(request.Name.ToLower()))
                && (request.EmployeeDesignationId == 0 || request.EmployeeDesignationId == null || x.EmployeeDesignationId == request.EmployeeDesignationId)
                && (x.EmployeeWorkSiteTypeId == request.EmployeeWorkSiteTypeId)
                && x.CreatedDate >= request.FDate.Value
                && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1);

            Expression<Func<Entities.Models.AspNetUsers, object>>[] includes = {
                x => x.AspNetUserRoles,
                x => x.Department,
                x => x.Department.Company,
                x => x.Store,
                x => x.Attachments,
                x => x.EmployeeDesignation,
            };

            List<string> thenIncludes = new()
            {
                "AspNetUserRoles.Role"
            };

            Expression<Func<Entities.Models.AspNetUsers, object>> OrderBy = null;
            Expression<Func<Entities.Models.AspNetUsers, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.AspNetUsers>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
           var users = mapper.Map<IEnumerable<GetAllUsers>>(entity.Item1.ToList()).ToList();

            foreach (var item in users)
            {
                List<string> roleids = new List<string>();
                List<string> rolenames = new List<string>();
                foreach (var role in item.AspNetUserRoles.ToList())
                {
                    roleids.Add(role.RoleId.ToString());
                    rolenames.Add(role.Role.Name);
                }
                item.RoleId = roleids.ToArray();
                item.RoleName = string.Join(", ", rolenames); // Comma-separated role names
               // item.AspNetUserRoles = null;
            }

            users = users.Where(y => request.Role == "" || y.RoleName.Contains(request.Role.Trim())).ToList();

            return new Tuple<List<GetAllUsers>, long>(users, entity.Item2);
        }


    }
}

