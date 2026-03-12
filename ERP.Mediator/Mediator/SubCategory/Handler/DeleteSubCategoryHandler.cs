using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Handler
{
    public class DeleteSubCategoryHandler : IRequestHandler<DeleteSubCategoryQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteSubCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteSubCategoryQuery request, CancellationToken cancellationToken)
        {
            var SubCategory = await unitOfWork.Repository<Entities.Models.SubCategory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            SubCategory.IsDelete = true;
            SubCategory.IsActive = false;
            SubCategory.DeleteDate = DateTime.Now;
            SubCategory.ModifiedDate = DateTime.Now;
            SubCategory.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.SubCategory>().Update(SubCategory);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
