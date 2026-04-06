using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.SugarType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SugarType.Handler
{
    public class SaveSugarTypeHandler : IRequestHandler<SaveSugarTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveSugarTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveSugarTypeCommand, long>.Handle(SaveSugarTypeCommand request, CancellationToken cancellationToken)
        {
            var SugarType = await unitOfWork.Repository<Entities.Models.SugarType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.SugarType>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (SugarType == null)
                {
                    var _SugarType = mapper.Map<Entities.Models.SugarType>(request);
                    _SugarType.CompanyId = sessionProvider.Session.CompanyId;
                    _SugarType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _SugarType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.SugarType>().Add(_SugarType);
                    SaveChanges();
                }
                else
                {
                    var _SugarType = mapper.Map<Entities.Models.SugarType>(request);
                    _SugarType.CompanyId = SugarType.CompanyId;
                    _SugarType.CreatedById = SugarType.CreatedById;
                    _SugarType.CreatedDate = SugarType.CreatedDate;
                    _SugarType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _SugarType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.SugarType>().Update(_SugarType);
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