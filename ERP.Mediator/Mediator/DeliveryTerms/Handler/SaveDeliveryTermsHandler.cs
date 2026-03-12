using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.DeliveryTerms.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Handler
{
    public class SaveDeliveryTermsHandler : IRequestHandler<SaveDeliveryTermsCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveDeliveryTermsHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveDeliveryTermsCommand, long>.Handle(SaveDeliveryTermsCommand request, CancellationToken cancellationToken)
        {
            var DeliveryTerms = await unitOfWork.Repository<Entities.Models.DeliveryTerms>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.DeliveryTerms>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (DeliveryTerms == null)
                {
                    var _DeliveryTerms = mapper.Map<Entities.Models.DeliveryTerms>(request);
                    _DeliveryTerms.CompanyId = sessionProvider.Session.CompanyId;
                    _DeliveryTerms.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _DeliveryTerms.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.DeliveryTerms>().Add(_DeliveryTerms);
                    SaveChanges();
                }
                else
                {
                    var _DeliveryTerms = mapper.Map<Entities.Models.DeliveryTerms>(request);
                    _DeliveryTerms.CompanyId = DeliveryTerms.CompanyId;
                    _DeliveryTerms.CreatedById = DeliveryTerms.CreatedById;
                    _DeliveryTerms.CreatedDate = DeliveryTerms.CreatedDate;
                    _DeliveryTerms.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _DeliveryTerms.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.DeliveryTerms>().Update(_DeliveryTerms);
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