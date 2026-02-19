using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.UOM.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UOM.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetUOMByIdQuery, GetUOM>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetUOM> Handle(GetUOMByIdQuery request, CancellationToken cancellationToken)
        {
            var UOM = await unitOfWork.Repository<Entities.Models.UOM>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _UOM = mapper.Map<GetUOM>(UOM);
            return _UOM;
        }
    }
}
