using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IndentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Handler
{
    public class DeleteIndentTypeHandler : IRequestHandler<DeleteIndentTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteIndentTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteIndentTypeQuery request, CancellationToken cancellationToken)
        {
            var IndentType = await unitOfWork.Repository<Entities.Models.IndentType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            IndentType.IsDelete = true;
            IndentType.IsActive = false;
            IndentType.DeleteDate = DateTime.Now;
            IndentType.ModifiedDate = DateTime.Now;
            IndentType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.IndentType>().Update(IndentType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
