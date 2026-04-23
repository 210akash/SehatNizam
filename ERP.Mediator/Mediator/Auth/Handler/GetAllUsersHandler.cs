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
    public class GetAllTemplatesHandler : IRequestHandler<GetAllUsersQuery, Tuple<List<GetAllUsers>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllTemplatesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        
        public async Task<Tuple<List<GetAllUsers>, long>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            //var users = await unitOfWork.Repository<global::ERP.Entities.Models.AspNetUsers>().GetAllAsync(null, null, "AspNetUserRoles,Department,Department.Company,Store");
            //var _user = mapper.Map<List<GetAllUsers>>(users);
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
            //return _user;

            Expression<Func<Entities.Models.AspNetUsers, bool>> predicate = x => 
                !x.IsDelete && x.IsEmployee &&
                (string.IsNullOrEmpty(request.Name)
                || x.FirstName.ToLower().Contains(request.Name.ToLower())
                || x.LastName.ToLower().Contains(request.Name.ToLower()))
                && (request.CNIC == "" || x.CNIC.Contains(request.CNIC))
                && (request.HrCode == "" || x.HrCode.Contains(request.HrCode))
                && (request.EmployeeWorkSiteTypeId == 0 || x.EmployeeWorkSiteTypeId == request.EmployeeWorkSiteTypeId)
                && (request.DepartmentId == 0 || x.DepartmentId == request.DepartmentId);

            Expression<Func<Entities.Models.AspNetUsers, object>>[] includes = {
                x => x.AspNetUserRoles,
                x => x.Department,
                x => x.Department.Company,
                x => x.Store,
                x => x.Attachments,
                x => x.EmployeeDocument,
                x => x.EmployeeWorkingDays,
                x => x.EmployeeBank,
                x => x.EmployeeType,
                x => x.EmployeeDesignation,
                x => x.EmployeeShift,
                x => x.City,
                x => x.EmployeeBank,
                x => x.EmployeeLeaveGroup,
                x => x.EmployeeEducation,
                x => x.UserProject,
                x => x.EmployeeWorkSiteType,
                x => x.EmployeeSalary.Where(y=>y.IsActive),
                x => x.EmployeeDevice.Where(y=>y.IsActive)
            };

            Expression<Func<Entities.Models.AspNetUsers, object>> OrderBy = null;
            Expression<Func<Entities.Models.AspNetUsers, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.AspNetUsers>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var users = mapper.Map<IEnumerable<GetAllUsers>>(entity.Item1.ToList()).ToList();

            foreach (var item in users)
            {
                List<string> roleids = new List<string>();
                List<long> projectIds = new List<long>();
                foreach (var role in item.AspNetUserRoles.ToList())
                {
                    roleids.Add(role.RoleId.ToString());
                }

                foreach (var userProject in item.UserProject)
                {
                    projectIds.Add(userProject.ProjectId);
                }

                item.RoleId = roleids.ToArray();
                item.ProjectIds = projectIds;
                item.AspNetUserRoles = null;
            }

            return new Tuple<List<GetAllUsers>, long>(users, entity.Item2);
        }


    }
}

