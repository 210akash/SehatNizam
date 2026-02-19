using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.GST.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GST.Handler
{
    public class GetCurrentGSTHandler : IRequestHandler<GetCurrentGSTQuery, GetGST>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCurrentGSTHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetGST> Handle(GetCurrentGSTQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date; // Use only the date part if time is irrelevant
            var GST = await unitOfWork.Repository<Entities.Models.GST>()
                .GetFirstAsNoTrackingAsync(y => y.FDate <= today && y.TDate >= today);
            var _GST = mapper.Map<GetGST>(GST);
            return _GST;
        }
    }
}
