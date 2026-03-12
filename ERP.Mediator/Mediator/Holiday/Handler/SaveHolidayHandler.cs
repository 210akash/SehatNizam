using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Holiday.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Holiday.Handler
{
    public class SaveHolidayHandler : IRequestHandler<SaveHolidayCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveHolidayHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveHolidayCommand, long>.Handle(SaveHolidayCommand request, CancellationToken cancellationToken)
        {
            var Holiday = await unitOfWork.Repository<Entities.Models.Holiday>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Holiday>().GetAsync(x => x.IsActive == true && x.IsDelete == false && x.Id != request.Id
                                 && x.Date == request.Date);

            if (checkDuplicate.Count() == 0)
            {
                if (Holiday == null)
                {
                    var _Holiday = mapper.Map<Entities.Models.Holiday>(request);
                    _Holiday.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Holiday.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Holiday>().Add(_Holiday);
                    SaveChanges();
                }
                else
                {
                    var _Holiday = mapper.Map<Entities.Models.Holiday>(request);
                    _Holiday.CreatedById = Holiday.CreatedById;
                    _Holiday.CreatedDate = Holiday.CreatedDate;
                    _Holiday.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Holiday.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Holiday>().Update(_Holiday);
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