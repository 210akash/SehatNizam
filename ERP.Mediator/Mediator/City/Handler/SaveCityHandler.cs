using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.City.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.City.Handler
{
    public class SaveCityHandler : IRequestHandler<SaveCityCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveCityHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveCityCommand, long>.Handle(SaveCityCommand request, CancellationToken cancellationToken)
        {
            var city = await unitOfWork.Repository<Entities.Models.City>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.City>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (city == null)
                {
                    var _city = mapper.Map<Entities.Models.City>(request);
                    _city.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _city.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.City>().Add(_city);
                    SaveChanges();
                }
                else
                {
                    var _city = mapper.Map<Entities.Models.City>(request);
                    _city.CreatedById = city.CreatedById;
                    _city.CreatedDate = city.CreatedDate;
                    _city.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _city.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.City>().Update(_city);
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