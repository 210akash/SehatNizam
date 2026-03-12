using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SalesTarget.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SalesTarget.Handler
{
    public class DeleteSalesTargetHandler : IRequestHandler<DeleteSalesTargetQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteSalesTargetHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteSalesTargetQuery request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.Repository<Entities.Models.SalesTarget>().GetExistsAsync(y => y.TargetMonth.Month == request.TargetMonth.Month ))
            {
                var salesTarget = await unitOfWork.Repository<Entities.Models.SalesTarget>().GetAsync(y => y.TargetMonth.Month == request.TargetMonth.Month );

                foreach (var item in salesTarget)
                {
                    item.IsDelete = true;
                    item.IsActive = false;
                    item.ModifiedDate = DateTime.Now;
                    item.DeleteDate = DateTime.Now;
                    item.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    unitOfWork.Repository<Entities.Models.SalesTarget>().Update(item);
                }
                
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
