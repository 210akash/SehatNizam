using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountSubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Handler
{
    public class DeleteAccountSubCategoryHandler : IRequestHandler<DeleteAccountSubCategoryQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteAccountSubCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAccountSubCategoryQuery request, CancellationToken cancellationToken)
        {
            var AccountSubCategory = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            AccountSubCategory.IsDelete = true;
            AccountSubCategory.IsActive = false;
            AccountSubCategory.DeleteDate = DateTime.Now;
            AccountSubCategory.ModifiedDate = DateTime.Now;
            AccountSubCategory.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.AccountSubCategory>().Update(AccountSubCategory);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
