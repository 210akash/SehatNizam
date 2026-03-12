using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Row.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Row.Handler
{
    public class DeleteRowHandler : IRequestHandler<DeleteRowQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRowHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteRowQuery request, CancellationToken cancellationToken)
        {
            if (!await unitOfWork.Repository<Entities.Models.Zone>().GetExistsAsync(y => y.Id == request.Id && y.IsActive))
            {
                var Row = await unitOfWork.Repository<Entities.Models.Row>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                Row.IsDelete = true;
                Row.IsActive = false;
                Row.ModifiedDate = DateTime.Now;
                Row.DeleteDate = DateTime.Now;
                Row.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.Row>().Update(Row);
                var check = await unitOfWork.SaveChangesAsync();
                if (check > 0)
                {
                    return (long)ResponseStatus.OK;
                }
                else
                {
                    return (long)ResponseStatus.Error;
                }
            }
            else
                return (long)ResponseStatus.Conflict;
        }
    }
}
