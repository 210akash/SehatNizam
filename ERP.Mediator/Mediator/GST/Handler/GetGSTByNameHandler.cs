using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.GST.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GST.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetGSTByNameQuery, List<GetGST>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetGST>> Handle(GetGSTByNameQuery request, CancellationToken cancellationToken)
        {
            var GST = await unitOfWork.Repository<Entities.Models.GST>().GetAsync();
            var _GST = mapper.Map<List<GetGST>>(GST);
            return _GST;
        }
    }
}
