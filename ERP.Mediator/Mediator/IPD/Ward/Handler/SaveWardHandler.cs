using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Ward.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Ward.Handler
{
    public class SaveWardHandler : IRequestHandler<SaveWardCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveWardHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveWardCommand request, CancellationToken cancellationToken)
        {
            var Ward = await unitOfWork.Repository<Entities.Models.Ward>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.Id);

            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Ward>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                               && x.IsActive == true
                               && x.IsDelete == false
                               && x.Id != request.Id
                               && x.ProjectId == sessionProvider.Session.SelectedWarehouseId);

            if (checkDuplicate.Count() == 0)
            {
                if (Ward == null)
                {
                    string _WardCode = "";
                    if (await unitOfWork.Repository<Entities.Models.Ward>()
                        .GetExistsAsync(y => y.ProjectId == sessionProvider.Session.SelectedWarehouseId && y.IsActive == true))
                    {
                        Func<IQueryable<Entities.Models.Ward>, IOrderedQueryable<Entities.Models.Ward>> OrderByDesc =
                            query => query.OrderByDescending(x => x.Code);
                        var WardCode = await unitOfWork.Repository<Entities.Models.Ward>()
                            .GetOneAsync(y => y.IsActive == true && y.ProjectId == sessionProvider.Session.SelectedWarehouseId, OrderByDesc, null);
                        int No = Convert.ToInt32(WardCode.Code) + 1;
                        _WardCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                    {
                        _WardCode = "01";
                    }

                    request.Code = _WardCode;

                    var _Ward = mapper.Map<Entities.Models.Ward>(request);
                    _Ward.ProjectId = sessionProvider.Session.SelectedWarehouseId;
                    _Ward.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Ward.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Ward>().Add(_Ward);
                    SaveChanges();
                }
                else
                {
                    var _Ward = mapper.Map<Entities.Models.Ward>(request);
                    _Ward.Code = Ward.Code;
                    _Ward.ProjectId = Ward.ProjectId;
                    _Ward.CreatedById = Ward.CreatedById;
                    _Ward.CreatedDate = Ward.CreatedDate;
                    _Ward.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Ward.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Ward>().Update(_Ward);
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