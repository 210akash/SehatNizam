using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IGPType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IGPType.Handler
{
    public class SaveIGPTypeHandler : IRequestHandler<SaveIGPTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveIGPTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveIGPTypeCommand, long>.Handle(SaveIGPTypeCommand request, CancellationToken cancellationToken)
        {
            var IGPType = await unitOfWork.Repository<Entities.Models.IGPType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.IGPType>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (IGPType == null)
                {
                    var _IGPType = mapper.Map<Entities.Models.IGPType>(request);
                    _IGPType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _IGPType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.IGPType>().Add(_IGPType);
                    SaveChanges();
                }
                else
                {
                    var _IGPType = mapper.Map<Entities.Models.IGPType>(request);
                    _IGPType.CreatedById = IGPType.CreatedById;
                    _IGPType.CreatedDate = IGPType.CreatedDate;
                    _IGPType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _IGPType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.IGPType>().Update(_IGPType);
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