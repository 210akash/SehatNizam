using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Store.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Handler
{
    public class SaveStoreHandler : IRequestHandler<SaveStoreCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveStoreHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveStoreCommand, long>.Handle(SaveStoreCommand request, CancellationToken cancellationToken)
        {
            var Store = await unitOfWork.Repository<Entities.Models.Store>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Store>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.Location.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (Store == null)
                {
                    var _Store = mapper.Map<Entities.Models.Store>(request);
                    _Store.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Store.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Store>().Add(_Store);
                    SaveChanges();
                }
                else
                {
                    var _Store = mapper.Map<Entities.Models.Store>(request);
                    _Store.CreatedById = Store.CreatedById;
                    _Store.CreatedDate = Store.CreatedDate;
                    _Store.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Store.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Store>().Update(_Store);
                    SaveChanges();
                }
                return 200;

            }
            else
            {
                return 409;
            }

        }
    }
}