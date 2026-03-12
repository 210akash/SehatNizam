using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Company.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Company.Handler
{
    public class DeleteCompanyHandler : IRequestHandler<DeleteCompanyQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteCompanyQuery request, CancellationToken cancellationToken)
        {
            var company = await unitOfWork.Repository<Entities.Models.Company>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            company.IsDelete = true;
            company.IsActive = false;
            company.DeleteDate = DateTime.Now;
            company.ModifiedDate = DateTime.Now;
            company.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Company>().Update(company);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
