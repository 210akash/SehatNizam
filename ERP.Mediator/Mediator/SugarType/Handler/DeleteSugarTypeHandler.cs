using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SugarType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SugarType.Handler
{
    public class DeleteSugarTypeHandler : IRequestHandler<DeleteSugarTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteSugarTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteSugarTypeQuery request, CancellationToken cancellationToken)
        {
            var SugarType = await unitOfWork.Repository<Entities.Models.SugarType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            SugarType.IsDelete = true;
            SugarType.IsActive = false;
            SugarType.DeleteDate = DateTime.Now;
            SugarType.ModifiedDate = DateTime.Now;
            SugarType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.SugarType>().Update(SugarType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
