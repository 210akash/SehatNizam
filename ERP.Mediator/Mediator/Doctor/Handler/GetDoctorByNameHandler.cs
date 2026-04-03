using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Doctor.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Account.Handler
{
    public class GetDoctorByNameHandler : IRequestHandler<GetDoctorByNameQuery, List<GetEmployee>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetDoctorByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployee>> Handle(GetDoctorByNameQuery request, CancellationToken cancellationToken)
        {
            var doctorrole = await unitOfWork.Repository<AspNetRoles>()
                .GetFirstAsNoTrackingAsync(x => x.Name == "Doctor");

            var result = await unitOfWork.Repository<AspNetUsers>()
             .GetQueryable()
             .Where(e =>
             e.IsActive &&
             e.DepartmentId == request.DepartmentId &&
             e.AspNetUserRoles.Any(y => y.RoleId == doctorrole.Id) &&
             (e.FirstName.Contains(request.Name) || e.LastName.Contains(request.Name)))
             .Take(10)
             .Select(e => new GetEmployee
             {
                 Id = e.Id,
                 HrCode = e.HrCode,
                 WorkLocation = e.WorkLocation,
                 PhoneNumber = e.PhoneNumber,
                 FirstName = e.FirstName,
                 LastName = e.LastName,
                 Designation = e.EmployeeDesignation.Name,
                 
             }).ToListAsync();

            return result;
        }
    }
}
