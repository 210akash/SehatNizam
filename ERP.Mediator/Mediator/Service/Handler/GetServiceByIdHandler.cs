using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Service.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Service.Handler
{
    public class GetServiceByIdHandler : IRequestHandler<GetServiceByIdQuery, GetService>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetServiceByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetService> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            Expression<System.Func<Entities.Models.Service, object>>[] includes = {
                x => x.Department
            };

            var service = await unitOfWork.Repository<Entities.Models.Service>().GetByIdAsync(request.Id, includes);
            if (service == null || service.IsDelete)
            {
                return null;
            }

            var result = mapper.Map<GetService>(service);
            
            if (service.Department != null)
            {
                result.DepartmentName = service.Department.Name;
            }

            return result;
        }
    }
}
