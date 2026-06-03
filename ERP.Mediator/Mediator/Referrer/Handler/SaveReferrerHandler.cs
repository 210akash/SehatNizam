using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Referrer.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Referrer.Handler
{
    public class SaveReferrerHandler : IRequestHandler<SaveReferrerCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveReferrerHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveReferrerCommand, long>.Handle(SaveReferrerCommand request, CancellationToken cancellationToken)
        {
            var Referrer = await unitOfWork.Repository<Entities.Models.Referrer>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Referrer>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.Hospital.ToLower().Trim() == request.Hospital.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (Referrer == null)
                {
                    var _Referrer = mapper.Map<Entities.Models.Referrer>(request);
                    _Referrer.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Referrer.CompanyId = sessionProvider.Session.CompanyId;
                    _Referrer.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Referrer>().Add(_Referrer);
                    SaveChanges();
                }
                else
                {
                    var _Referrer = mapper.Map<Entities.Models.Referrer>(request);
                    _Referrer.CreatedById = Referrer.CreatedById;
                    _Referrer.CreatedDate = Referrer.CreatedDate;
                    _Referrer.CompanyId = sessionProvider.Session.CompanyId;
                    _Referrer.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Referrer.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Referrer>().Update(_Referrer);
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