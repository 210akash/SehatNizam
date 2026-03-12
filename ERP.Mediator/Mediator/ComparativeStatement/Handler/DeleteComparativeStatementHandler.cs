using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ComparativeStatement.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Handler
{
    public class DeleteComparativeStatementHandler : IRequestHandler<DeleteComparativeStatementQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteComparativeStatementHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteComparativeStatementQuery request, CancellationToken cancellationToken)
        {
            var ComparativeStatement = await unitOfWork.Repository<Entities.Models.ComparativeStatement>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            ComparativeStatement.IsDelete = true;
            ComparativeStatement.IsActive = false;
            ComparativeStatement.DeleteDate = DateTime.Now;
            ComparativeStatement.ModifiedDate = DateTime.Now;
            ComparativeStatement.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.ComparativeStatement>().Update(ComparativeStatement);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
