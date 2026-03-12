using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.GST.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GST.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetGSTByIdQuery, GetGST>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetGST> Handle(GetGSTByIdQuery request, CancellationToken cancellationToken)
        {
            var GST = await unitOfWork.Repository<Entities.Models.GST>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _GST = mapper.Map<GetGST>(GST);
            return _GST;
        }
    }
}
