using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PaymentMode.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Handler
{
    public class SavePaymentModeHandler : IRequestHandler<SavePaymentModeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SavePaymentModeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SavePaymentModeCommand, long>.Handle(SavePaymentModeCommand request, CancellationToken cancellationToken)
        {
            var PaymentMode = await unitOfWork.Repository<Entities.Models.PaymentMode>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.PaymentMode>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (PaymentMode == null)
                {
                    var _PaymentMode = mapper.Map<Entities.Models.PaymentMode>(request);
                    _PaymentMode.CompanyId = sessionProvider.Session.CompanyId;
                    _PaymentMode.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _PaymentMode.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.PaymentMode>().Add(_PaymentMode);
                    SaveChanges();
                }
                else
                {
                    var _PaymentMode = mapper.Map<Entities.Models.PaymentMode>(request);
                    _PaymentMode.CompanyId = PaymentMode.CompanyId;
                    _PaymentMode.CreatedById = PaymentMode.CreatedById;
                    _PaymentMode.CreatedDate = PaymentMode.CreatedDate;
                    _PaymentMode.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _PaymentMode.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.PaymentMode>().Update(_PaymentMode);
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