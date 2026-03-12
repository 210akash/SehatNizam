using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.HRYear.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.HRYear.Handler
{
    public class DeleteHRYearHandler : IRequestHandler<DeleteHRYearQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteHRYearHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteHRYearQuery request, CancellationToken cancellationToken)
        {
            var HRYear = await unitOfWork.Repository<Entities.Models.HRYear>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            HRYear.IsDelete = true;
            HRYear.IsActive = false;
            HRYear.DeleteDate = DateTime.Now;
            HRYear.ModifiedDate = DateTime.Now;
            HRYear.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.HRYear>().Update(HRYear);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
