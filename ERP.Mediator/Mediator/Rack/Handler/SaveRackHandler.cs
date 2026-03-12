using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Rack.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Rack.Handler
{
    public class SaveRackHandler : IRequestHandler<SaveRackCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRackHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveRackCommand, long>.Handle(SaveRackCommand request, CancellationToken cancellationToken)
        {
            var Rack = await unitOfWork.Repository<Entities.Models.Rack>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            //var checkDuplicate = await unitOfWork.Repository<Entities.Models.Rack>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            //if (checkDuplicate.Count() == 0)
            //{
            if (Rack == null)
            {
                var _Rack = mapper.Map<Entities.Models.Rack>(request);
                _Rack.CreatedById = sessionProvider.Session.LoggedInUserId;
                _Rack.CompanyId = sessionProvider.Session.CompanyId;
                _Rack.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Rack>().Add(_Rack);
                SaveChanges();
            }
            else
            {
                var _Rack = mapper.Map<Entities.Models.Rack>(request);
                _Rack.CreatedById = Rack.CreatedById;
                _Rack.CreatedDate = Rack.CreatedDate;
                _Rack.CompanyId = sessionProvider.Session.CompanyId;
                _Rack.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _Rack.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Rack>().Update(_Rack);
                SaveChanges();
            }
            return 200;
        }
    }
}