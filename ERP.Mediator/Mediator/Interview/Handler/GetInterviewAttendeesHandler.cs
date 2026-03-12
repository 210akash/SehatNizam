using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ERP.BusinessModels.ResponseVM;
using ERP.Repositories.UnitOfWork;
using System.Linq.Expressions;
using ERP.Mediator.Mediator.Interview.Query;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class GetInterviewAttendeesHandler : IRequestHandler<GetInterviewAttendeesQuery, List<GetAllUsers>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetInterviewAttendeesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        
        public async Task<List<GetAllUsers>> Handle(GetInterviewAttendeesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.AspNetUsers, bool>> predicate = x => x.IsActive && !x.IsDelete;

            Expression<Func<Entities.Models.AspNetUsers, object>>[] includes = {
                x => x.AspNetUserRoles,
                x => x.Department,
                x => x.Department.Company,
                x => x.EmployeeType,
                x => x.EmployeeDesignation,
            };

            Expression<Func<Entities.Models.AspNetUsers, object>> OrderBy = null;
            Expression<Func<Entities.Models.AspNetUsers, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.AspNetUsers>().GetPagingWhereAsNoTrackingAsync(predicate, null, OrderBy, OrderByDesc, null, includes);
            var users = mapper.Map<IEnumerable<GetAllUsers>>(entity.Item1.ToList()).ToList();

            foreach (var item in users)
            {
                List<string> roleids = new List<string>();
                foreach (var role in item.AspNetUserRoles.ToList())
                {
                    roleids.Add(role.RoleId.ToString());
                }

                item.RoleId = roleids.ToArray();
                item.AspNetUserRoles = null;
            }

            return users;
        }


    }
}

