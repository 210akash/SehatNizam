using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.VisitType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.VisitType.Handler
{
    public class SaveVisitTypeHandler : IRequestHandler<SaveVisitTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveVisitTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveVisitTypeCommand, long>.Handle(SaveVisitTypeCommand request, CancellationToken cancellationToken)
        {
            var VisitType = await unitOfWork.Repository<Entities.Models.VisitType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.VisitType>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (VisitType == null)
                {
                    var _VisitType = mapper.Map<Entities.Models.VisitType>(request);
                    _VisitType.CompanyId = sessionProvider.Session.CompanyId;
                    _VisitType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _VisitType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.VisitType>().Add(_VisitType);
                    SaveChanges();
                }
                else
                {
                    var _VisitType = mapper.Map<Entities.Models.VisitType>(request);
                    _VisitType.CompanyId = VisitType.CompanyId;
                    _VisitType.CreatedById = VisitType.CreatedById;
                    _VisitType.CreatedDate = VisitType.CreatedDate;
                    _VisitType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _VisitType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.VisitType>().Update(_VisitType);
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