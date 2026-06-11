using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.AdvancePayments.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AdvancePayments.Handler
{
    public class GetAllAdvancePaymentsHandler : IRequestHandler<GetAllAdvancePaymentsQuery, Tuple<IEnumerable<GetAppointmentPayment>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllAdvancePaymentsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetAppointmentPayment>, long>> Handle(GetAllAdvancePaymentsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.AppointmentPayment, bool>> predicate = x => x.IsActive == true && x.IsDelete == false
            && x.AppointmentId == request.AppointmentId;

            Expression<Func<Entities.Models.AppointmentPayment, object>>[] includes = {
                x => x.Service,
                x => x.PaymentMode,
                x => x.PaymentStatus
            };

            Expression<Func<Entities.Models.AppointmentPayment, object>> OrderBy = null;
            Expression<Func<Entities.Models.AppointmentPayment, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.AppointmentPayment>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var Project = mapper.Map<IEnumerable<GetAppointmentPayment>>(entity.Item1.ToList());
            return new Tuple<IEnumerable<GetAppointmentPayment>, long>(Project, entity.Item2);
        }
    }
}
