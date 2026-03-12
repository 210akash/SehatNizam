using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Row.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Row.Handler
{
    public class GetRowByNameHandler : IRequestHandler<GetRowByNameQuery, List<GetRow>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRowByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRow>> Handle(GetRowByNameQuery request, CancellationToken cancellationToken)
        {
            var Row = await unitOfWork.Repository<Entities.Models.Row>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _Row = mapper.Map<List<GetRow>>(Row);
            return _Row;
        }
    }
}
