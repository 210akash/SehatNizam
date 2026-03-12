using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.HRYear.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.HRYear.Handler
{
    public class SaveHRYearHandler : IRequestHandler<SaveHRYearCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveHRYearHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveHRYearCommand, long>.Handle(SaveHRYearCommand request, CancellationToken cancellationToken)
        {
            var HRYear = await unitOfWork.Repository<Entities.Models.HRYear>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.HRYear>().GetAsync(x => x.IsActive == true && x.IsDelete == false && x.Id != request.Id
                                 && x.StartDate == request.StartDate && x.EndDate == request.EndDate);

            if (checkDuplicate.Count() == 0)
            {
                if (HRYear == null)
                {
                    var _HRYear = mapper.Map<Entities.Models.HRYear>(request);
                    _HRYear.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _HRYear.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.HRYear>().Add(_HRYear);
                    SaveChanges();
                }
                else
                {
                    var _HRYear = mapper.Map<Entities.Models.HRYear>(request);
                    _HRYear.CreatedById = HRYear.CreatedById;
                    _HRYear.CreatedDate = HRYear.CreatedDate;
                    _HRYear.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _HRYear.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.HRYear>().Update(_HRYear);
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