using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountFlow.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountFlow.Handler
{
    public class SaveAccountFlowHandler : IRequestHandler<SaveAccountFlowCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAccountFlowHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
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

        public async Task<long> Handle(SaveAccountFlowCommand request, CancellationToken cancellationToken)
        {
            var AccountFlow = await unitOfWork.Repository<Entities.Models.AccountFlow>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            var checkDuplicate = await unitOfWork.Repository<Entities.Models.AccountFlow>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                               && x.IsActive == true
                               && x.IsDelete == false
                               && x.Id != request.Id
                               && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (AccountFlow == null)
                {
                    string _AccountFlowCode = "";
                    if (await unitOfWork.Repository<Entities.Models.AccountFlow>()
                        .GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.IsActive == true))
                    {
                        Func<IQueryable<Entities.Models.AccountFlow>, IOrderedQueryable<Entities.Models.AccountFlow>> OrderByDesc =
                            query => query.OrderByDescending(x => x.Code);
                        var AccountFlowCode = await unitOfWork.Repository<Entities.Models.AccountFlow>()
                            .GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                        int No = Convert.ToInt32(AccountFlowCode.Code) + 1;
                        _AccountFlowCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                    {
                        _AccountFlowCode = "01";
                    }

                    request.Code = _AccountFlowCode;

                    var _AccountFlow = mapper.Map<Entities.Models.AccountFlow>(request);
                    _AccountFlow.CompanyId = sessionProvider.Session.CompanyId;
                    _AccountFlow.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _AccountFlow.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountFlow>().Add(_AccountFlow);
                    SaveChanges();
                }
                else
                {
                    var _AccountFlow = mapper.Map<Entities.Models.AccountFlow>(request);
                    _AccountFlow.Code = AccountFlow.Code;
                    _AccountFlow.CompanyId = AccountFlow.CompanyId;
                    _AccountFlow.CreatedById = AccountFlow.CreatedById;
                    _AccountFlow.CreatedDate = AccountFlow.CreatedDate;
                    _AccountFlow.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _AccountFlow.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountFlow>().Update(_AccountFlow);
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