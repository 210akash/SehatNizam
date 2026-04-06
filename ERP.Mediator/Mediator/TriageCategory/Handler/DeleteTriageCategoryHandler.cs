using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.TriageCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.TriageCategory.Handler
{
    public class DeleteTriageCategoryHandler : IRequestHandler<DeleteTriageCategoryQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteTriageCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteTriageCategoryQuery request, CancellationToken cancellationToken)
        {
            var TriageCategory = await unitOfWork.Repository<Entities.Models.TriageCategory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            TriageCategory.IsDelete = true;
            TriageCategory.IsActive = false;
            TriageCategory.DeleteDate = DateTime.Now;
            TriageCategory.ModifiedDate = DateTime.Now;
            TriageCategory.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.TriageCategory>().Update(TriageCategory);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
