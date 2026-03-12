using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Section.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Section.Handler
{
    public class DeleteSectionHandler : IRequestHandler<DeleteSectionQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteSectionHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteSectionQuery request, CancellationToken cancellationToken)
        {
            if (!await unitOfWork.Repository<Entities.Models.Zone>().GetExistsAsync(y => y.Id == request.Id && y.IsActive))
            {
                var Section = await unitOfWork.Repository<Entities.Models.Section>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                Section.IsDelete = true;
                Section.IsActive = false;
                Section.ModifiedDate = DateTime.Now;
                Section.DeleteDate = DateTime.Now;
                Section.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.Section>().Update(Section);
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
