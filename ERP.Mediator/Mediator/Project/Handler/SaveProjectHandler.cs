using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Project.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
                    foreach (var item in request.StoreIds)
                    {
                        ProjectStore lObjProjectStore = new()
                        {
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            ProjectId = _Project.Id,
                            StoreId = item
                        };
                        unitOfWork.Repository<ProjectStore>().Add(lObjProjectStore);
                    }
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

                    var ProjectStoreList = await unitOfWork.Repository<ProjectStore>()
                        .GetPagingWhereAsNoTrackingAsync(y => y.ProjectId == request.Id && y.IsActive == true,
                        null, null, null, null, null).Item1.ToListAsync();

                    List<long> previousProjectStoreIds = ProjectStoreList
                        .Select(y => y.StoreId)
                        .ToList();

                    List<long> currentProjectStoreIds = request.StoreIds;
                    List<long> deletedProjectStoreIds = previousProjectStoreIds.Except(currentProjectStoreIds).ToList();
                    List<long> addProjectStoreIds = currentProjectStoreIds.Except(previousProjectStoreIds).ToList();

                    // Handle deletions
                    foreach (var deletedProjectStoreId in deletedProjectStoreIds)
                    {
                        ProjectStore projectStore = ProjectStoreList.Where(y => y.StoreId == deletedProjectStoreId).FirstOrDefault();

                        if (projectStore != null)
                        {
                            projectStore.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            projectStore.ModifiedDate = DateTime.Now;
                            projectStore.IsActive = false; // Soft delete
                            projectStore.IsDelete = true; // Soft delete
                            unitOfWork.Repository<ProjectStore>().Update(projectStore);
                            SaveChanges();
                        }
                    }

                    // Handle additions
                    foreach (var storeId in addProjectStoreIds)
                    {
                        ProjectStore lObjProjectStore = new()
                        {
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            ProjectId = request.Id,
                            StoreId = storeId
                        };
                        unitOfWork.Repository<ProjectStore>().Add(lObjProjectStore);
                        SaveChanges();
                    }
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