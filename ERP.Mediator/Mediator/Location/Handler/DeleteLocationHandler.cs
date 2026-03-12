using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Location.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Location.Handler
{
    public class DeleteLocationHandler : IRequestHandler<DeleteLocationQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteLocationHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteLocationQuery request, CancellationToken cancellationToken)
        {
            var Location = await unitOfWork.Repository<Entities.Models.Location>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Location.IsDelete = true;
            Location.IsActive = false;
            Location.DeleteDate = DateTime.Now;
            Location.ModifiedDate = DateTime.Now;
            Location.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Location>().Update(Location);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
