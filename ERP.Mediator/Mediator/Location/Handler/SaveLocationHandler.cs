using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Location.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Location.Handler
{
    public class SaveLocationHandler : IRequestHandler<SaveLocationCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveLocationHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveLocationCommand, long>.Handle(SaveLocationCommand request, CancellationToken cancellationToken)
        {
            var Location = await unitOfWork.Repository<Entities.Models.Location>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Location>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == request.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (Location == null)
                {
                    var _Location = mapper.Map<Entities.Models.Location>(request);
                    _Location.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Location.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Location>().Add(_Location);
                    SaveChanges();
                }
                else
                {
                    var _Location = mapper.Map<Entities.Models.Location>(request);
                    _Location.CreatedById = Location.CreatedById;
                    _Location.CreatedDate = Location.CreatedDate;
                    _Location.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Location.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Location>().Update(_Location);
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