using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RadiologyOrder.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyOrder.Handler
{
    public class SaveRadiologyOrderHandler : IRequestHandler<SaveRadiologyOrderCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public SaveRadiologyOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SaveRadiologyOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.AppointmentId <= 0 || request.RadiologyTypeId <= 0 || request.StatusId <= 0)
            {
                return 400;
            }

            Entities.Models.RadiologyOrder radiologyOrder;

            if (request.Id > 0)
            {
                radiologyOrder = await unitOfWork.Repository<Entities.Models.RadiologyOrder>().FindAsync(y => y.Id == request.Id);
                if (radiologyOrder == null)
                {
                    return 404;
                }

                mapper.Map(request, radiologyOrder);
                radiologyOrder.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
                radiologyOrder.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.RadiologyOrder>().Update(radiologyOrder);
            }
            else
            {
                radiologyOrder = mapper.Map<Entities.Models.RadiologyOrder>(request);
                radiologyOrder.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                radiologyOrder.IsActive = true;

                await unitOfWork.Repository<Entities.Models.RadiologyOrder>().AddAsync(radiologyOrder);
            }

            await unitOfWork.SaveChangesAsync();
            return 200;
        }
    }
}
