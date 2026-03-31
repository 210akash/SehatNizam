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
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.Doctor.Handler
{
    public class GetAllDoctorsHandler : IRequestHandler<GetAllDoctorsQuery, Tuple<List<GetAllUsers>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllDoctorsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<List<GetAllUsers>, long>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
        {
            var doctorrole = await unitOfWork.Repository<Entities.Models.AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name.ToLower() == "doctor");
            Expression<Func<Entities.Models.AspNetUsers, bool>> predicate = x => x.IsActive
                && (request.DepartmentId == 0 || request.DepartmentId == null || x.DepartmentId == request.DepartmentId)
                && x.UserProject.Any(y => y.ProjectId == sessionProvider.Session.SelectedWarehouseId)
                && x.AspNetUserRoles.Any(y => y.RoleId == doctorrole.Id)
                && (string.IsNullOrEmpty(request.Name) || x.FirstName.ToLower().Contains(request.Name.ToLower()) || x.LastName.ToLower().Contains(request.Name.ToLower()))
                && (request.EmployeeDesignationId == 0 || request.EmployeeDesignationId == null || x.EmployeeDesignationId == request.EmployeeDesignationId);

            Expression<Func<Entities.Models.AspNetUsers, object>>[] includes = {
                x => x.AspNetUserRoles,
                x => x.Department,
                x => x.UserProject,
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
            var users = mapper.Map<IEnumerable<GetAllUsers>>(entity.Item1).ToList();
            return new Tuple<List<GetAllUsers>, long>(users, entity.Item2);
        }
    }
}

