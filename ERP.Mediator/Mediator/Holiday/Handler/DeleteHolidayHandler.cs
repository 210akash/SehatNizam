using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Holiday.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Holiday.Handler
{
    public class DeleteHolidayHandler : IRequestHandler<DeleteHolidayQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteHolidayHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteHolidayQuery request, CancellationToken cancellationToken)
        {
            var Holiday = await unitOfWork.Repository<Entities.Models.Holiday>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Holiday.IsDelete = true;
            Holiday.IsActive = false;
            Holiday.DeleteDate = DateTime.Now;
            Holiday.ModifiedDate = DateTime.Now;
            Holiday.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Holiday>().Update(Holiday);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
