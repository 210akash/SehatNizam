using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.LabOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.LabOrder.Handler
{
    public class GetAllLabOrderHandler : IRequestHandler<GetAllLabOrderQuery, Tuple<IEnumerable<GetLabOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllLabOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetLabOrder>, long>> Handle(GetAllLabOrderQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.LabOrder, bool>> predicate =
                x => x.IsActive == true
                && (!request.AppointmentId.HasValue || x.AppointmentId == request.AppointmentId.Value);

            Expression<Func<Entities.Models.LabOrder, object>>[] includes = { x => x.Status, x => x.LabOrderType , x => x.Appointment, x => x.Appointment.Patient };
            var result = unitOfWork.Repository<Entities.Models.LabOrder>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, x => x.Id, null, includes);
            return new Tuple<IEnumerable<GetLabOrder>, long>(mapper.Map<IEnumerable<GetLabOrder>>(result.Item1), result.Item2);
        }
    }
}
