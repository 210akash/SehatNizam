using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.GST.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GST.Handler
{
    public class SaveGSTHandler : IRequestHandler<SaveGSTCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveGSTHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveGSTCommand, long>.Handle(SaveGSTCommand request, CancellationToken cancellationToken)
        {
            var GST = await unitOfWork.Repository<Entities.Models.GST>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.GST>().GetAsync(x =>  x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (GST == null)
                {
                    var _GST = mapper.Map<Entities.Models.GST>(request);
                    _GST.CompanyId = sessionProvider.Session.CompanyId;
                    _GST.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _GST.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.GST>().Add(_GST);
                    SaveChanges();
                }
                else
                {
                    var _GST = mapper.Map<Entities.Models.GST>(request);
                    _GST.CompanyId = GST.CompanyId;
                    _GST.CreatedById = GST.CreatedById;
                    _GST.CreatedDate = GST.CreatedDate;
                    _GST.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _GST.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.GST>().Update(_GST);
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