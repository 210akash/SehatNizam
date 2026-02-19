using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.UOM.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UOM.Handler
{
    public class SaveUOMHandler : IRequestHandler<SaveUOMCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveUOMHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveUOMCommand, long>.Handle(SaveUOMCommand request, CancellationToken cancellationToken)
        {
            var UOM = await unitOfWork.Repository<Entities.Models.UOM>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.UOM>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (UOM == null)
                {
                    var _UOM = mapper.Map<Entities.Models.UOM>(request);
                    _UOM.CompanyId = sessionProvider.Session.CompanyId;
                    _UOM.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _UOM.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.UOM>().Add(_UOM);
                    SaveChanges();
                }
                else
                {
                    var _UOM = mapper.Map<Entities.Models.UOM>(request);
                    _UOM.CompanyId = UOM.CompanyId;
                    _UOM.CreatedById = UOM.CreatedById;
                    _UOM.CreatedDate = UOM.CreatedDate;
                    _UOM.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _UOM.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.UOM>().Update(_UOM);
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