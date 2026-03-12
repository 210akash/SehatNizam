using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Interview.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Interview.Handler
{
    public class DeleteInterviewHandler : IRequestHandler<DeleteInterviewQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteInterviewHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteInterviewQuery request, CancellationToken cancellationToken)
        {
            var interview = await unitOfWork.Repository<Entities.Models.Interview>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            interview.IsDelete = true;
            interview.IsActive = false;
            interview.DeleteDate = DateTime.Now;
            interview.ModifiedDate = DateTime.Now;
            interview.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Interview>().Update(interview);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
