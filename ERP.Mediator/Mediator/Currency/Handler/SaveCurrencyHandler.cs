using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Currency.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Currency.Handler
{
    public class SaveCurrencyHandler : IRequestHandler<SaveCurrencyCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveCurrencyHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveCurrencyCommand, long>.Handle(SaveCurrencyCommand request, CancellationToken cancellationToken)
        {
            var Currency = await unitOfWork.Repository<Entities.Models.Currency>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Currency>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (Currency == null)
                {
                    var _Currency = mapper.Map<Entities.Models.Currency>(request);
                    _Currency.CompanyId = sessionProvider.Session.CompanyId;
                    _Currency.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Currency.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Currency>().Add(_Currency);
                    SaveChanges();
                }
                else
                {
                    var _Currency = mapper.Map<Entities.Models.Currency>(request);
                    _Currency.CompanyId = Currency.CompanyId;
                    _Currency.CreatedById = Currency.CreatedById;
                    _Currency.CreatedDate = Currency.CreatedDate;
                    _Currency.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Currency.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Currency>().Update(_Currency);
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