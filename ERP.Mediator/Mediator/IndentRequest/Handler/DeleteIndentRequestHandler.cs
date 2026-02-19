using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IndentRequest.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Handler
{
    public class DeleteIndentRequestHandler : IRequestHandler<DeleteIndentRequestQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteIndentRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteIndentRequestQuery request, CancellationToken cancellationToken)
        {
            var IndentRequest = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            IndentRequest.IsDelete = true;
            IndentRequest.IsActive = false;
            IndentRequest.DeleteDate = DateTime.Now;
            IndentRequest.ModifiedDate = DateTime.Now;
            IndentRequest.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.IndentRequest>().Update(IndentRequest);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
