using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Project.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Project.Handler
{
    public class SaveProjectHandler : IRequestHandler<SaveProjectCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveProjectHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveProjectCommand, long>.Handle(SaveProjectCommand request, CancellationToken cancellationToken)
        {
            var Project = await unitOfWork.Repository<Entities.Models.Project>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Project>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == request.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (Project == null)
                {
                    var _Project = mapper.Map<Entities.Models.Project>(request);
                    _Project.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Project.CompanyId = sessionProvider.Session.CompanyId;
                    _Project.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Project>().Add(_Project);
                    SaveChanges();
                }
                else
                {
                    var _Project = mapper.Map<Entities.Models.Project>(request);
                    _Project.CreatedById  = Project.CreatedById;
                    _Project.CreatedDate  = Project.CreatedDate;
                    _Project.CompanyId    = Project.CompanyId;
                    _Project.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Project.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Project>().Update(_Project);
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