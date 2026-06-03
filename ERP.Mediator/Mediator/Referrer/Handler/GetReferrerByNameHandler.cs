using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Referrer.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Referrer.Handler
{
    public class GetReferrerByNameHandler : IRequestHandler<GetReferrerByNameQuery, List<GetReferrer>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetReferrerByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetReferrer>> Handle(GetReferrerByNameQuery request, CancellationToken cancellationToken)
        {
            var ReferBy = await unitOfWork.Repository<Entities.Models.Referrer>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _ReferBy = mapper.Map<List<GetReferrer>>(ReferBy).Take(10).ToList();
            return _ReferBy;
        }
    }
}
