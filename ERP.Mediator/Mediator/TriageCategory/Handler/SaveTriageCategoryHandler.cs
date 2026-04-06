using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.TriageCategory.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.TriageCategory.Handler
{
    public class SaveTriageCategoryHandler : IRequestHandler<SaveTriageCategoryCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveTriageCategoryHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            try
            {
                return unitOfWork.SaveChanges();

            }
            catch (Exception dex)
            {

                throw;
            }
        }

        public async Task<long> Handle(SaveTriageCategoryCommand request, CancellationToken cancellationToken)
        {
            var TriageCategory = await unitOfWork.Repository<Entities.Models.TriageCategory>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            var checkDuplicate = await unitOfWork.Repository<Entities.Models.TriageCategory>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                               && x.IsActive == true
                               && x.IsDelete == false
                               && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (TriageCategory == null)
                {
                    var _TriageCategory = mapper.Map<Entities.Models.TriageCategory>(request);
                    _TriageCategory.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _TriageCategory.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.TriageCategory>().Add(_TriageCategory);
                    SaveChanges();
                }
                else
                {
                    var _TriageCategory = mapper.Map<Entities.Models.TriageCategory>(request);
                    _TriageCategory.CreatedById = TriageCategory.CreatedById;
                    _TriageCategory.CreatedDate = TriageCategory.CreatedDate;
                    _TriageCategory.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _TriageCategory.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.TriageCategory>().Update(_TriageCategory);
                    SaveChanges();
                }

                return 200; // Success code for adding/updating
            }
            else
            {
                return 409; // Conflict code for duplicate
            }
        }
    }
}