using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AppointmentType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AppointmentType.Handler
{
    public class SaveAppointmentTypeHandler : IRequestHandler<SaveAppointmentTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAppointmentTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
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

        public async Task<long> Handle(SaveAppointmentTypeCommand request, CancellationToken cancellationToken)
        {
            var AppointmentType = await unitOfWork.Repository<Entities.Models.AppointmentType>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            var checkDuplicate = await unitOfWork.Repository<Entities.Models.AppointmentType>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                               && x.IsActive == true
                               && x.IsDelete == false
                               && x.Id != request.Id
                               && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (AppointmentType == null)
                {
                    string _AppointmentTypeCode = "";
                    if (await unitOfWork.Repository<Entities.Models.AppointmentType>()
                        .GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.IsActive == true))
                    {
                        Func<IQueryable<Entities.Models.AppointmentType>, IOrderedQueryable<Entities.Models.AppointmentType>> OrderByDesc =
                            query => query.OrderByDescending(x => x.Code);
                        var AppointmentTypeCode = await unitOfWork.Repository<Entities.Models.AppointmentType>()
                            .GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                        int No = Convert.ToInt32(AppointmentTypeCode.Code) + 1;
                        _AppointmentTypeCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                    {
                        _AppointmentTypeCode = "01";
                    }

                    request.Code = _AppointmentTypeCode;

                    var _AppointmentType = mapper.Map<Entities.Models.AppointmentType>(request);
                    _AppointmentType.CompanyId = sessionProvider.Session.CompanyId;
                    _AppointmentType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _AppointmentType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AppointmentType>().Add(_AppointmentType);
                    SaveChanges();
                }
                else
                {
                    var _AppointmentType = mapper.Map<Entities.Models.AppointmentType>(request);
                    _AppointmentType.Code = AppointmentType.Code;
                    _AppointmentType.CompanyId = AppointmentType.CompanyId;
                    _AppointmentType.CreatedById = AppointmentType.CreatedById;
                    _AppointmentType.CreatedDate = AppointmentType.CreatedDate;
                    _AppointmentType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _AppointmentType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AppointmentType>().Update(_AppointmentType);
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