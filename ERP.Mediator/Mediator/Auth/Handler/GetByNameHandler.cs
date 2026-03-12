using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Auth.Query;
using ERP.Repositories.UnitOfWork;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class GetByNameHandler : IRequestHandler<GetByNameQuery, List<GetAllUsers>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        
        public async Task<List<GetAllUsers>> Handle(GetByNameQuery request, CancellationToken cancellationToken)
        {
            var Account = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetAsync(y =>
            (y.FirstName.ToLower().Contains(request.name.ToLower()) ||
             y.LastName.ToLower().Contains(request.name.ToLower())) &&
             y.IsActive == true, null, null, "Department,EmployeeDocument,EmployeeDesignation");
            var users = mapper.Map<List<GetAllUsers>>(Account);
            return users;
        }
    }
}

