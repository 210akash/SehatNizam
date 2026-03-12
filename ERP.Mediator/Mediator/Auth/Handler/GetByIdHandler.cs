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
    public class GetByIdHandler : IRequestHandler<GetByIdQuery, GetAllUsers>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        
        public async Task<GetAllUsers> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.AspNetUsers, bool>> predicate = x =>
               x.Id == request.UserId;

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
                x => x.EmployeeDevice.Where(y=>y.IsActive)
            };

            string includeProperties = "AspNetUserRoles,Department,Department.Company,Store,Attachments,EmployeeDocument,EmployeeWorkingDays,EmployeeBank,EmployeeType,EmployeeDesignation,EmployeeShift,City,EmployeeLeaveGroup,EmployeeEducation,UserProject,EmployeeDevice";


            Expression<Func<Entities.Models.AspNetUsers, object>> OrderBy = null;
            Expression<Func<Entities.Models.AspNetUsers, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetFirstAsync(predicate,null, null, includeProperties);
            var users = mapper.Map<GetAllUsers>(entity);
            return users;
        }
    }
}

