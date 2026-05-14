using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.LabOrderType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.LabOrderType.Handler
{
    public class SaveLabTestVariableHandler : IRequestHandler<SaveLabTestVariableCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveLabTestVariableHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SaveLabTestVariableCommand request, CancellationToken cancellationToken)
        {
            if (request.LabOrderTypeId <= 0 || request.Variables == null || !request.Variables.Any())
                return 400;

            var userId = this.sessionProvider.Session.LoggedInUserId;

            var repo = unitOfWork.Repository<Entities.Models.LabTestVariable>();

            var existingVariables = await repo.FindAllAsync(x =>
                x.LabOrderTypeId == request.LabOrderTypeId &&
                x.IsActive && !x.IsDelete);

            var existingDict = existingVariables.ToDictionary(x => x.Id);

            var requestIds = request.Variables.Where(x => x.Id > 0).Select(x => x.Id).ToHashSet();

            // -----------------------------
            // 1. Soft Delete Removed Items
            // -----------------------------
            var toDelete = existingVariables
                .Where(x => !requestIds.Contains(x.Id))
                .ToList();

            foreach (var item in toDelete)
            {
                item.IsActive = false;
                item.IsDelete = true;
                item.DeleteDate = DateTime.Now;
                item.ModifiedById = userId;

                repo.Update(item);
            }

            // -----------------------------
            // 2. Add / Update Variables
            // -----------------------------
            foreach (var dto in request.Variables)
            {
                // UPDATE
                if (dto.Id > 0 && existingDict.TryGetValue(dto.Id, out var existing))
                {
                    existing.Name = dto.Name;
                    existing.Unit = dto.Unit;
                    existing.MaleMin = dto.MaleMin;
                    existing.MaleMax = dto.MaleMax;
                    existing.FemaleMin = dto.FemaleMin;
                    existing.FemaleMax = dto.FemaleMax;
                    existing.HasGenderRange = dto.HasGenderRange;
                    existing.DisplayOrder = dto.DisplayOrder;
                    existing.ResultType = dto.ResultType;

                    existing.ModifiedById = userId;
                    existing.ModifiedDate = DateTime.Now;

                    repo.Update(existing);

                    // OPTIONAL: update options
                    UpdateOptions(existing, dto);
                }
                // INSERT
                else
                {
                    var entity = new Entities.Models.LabTestVariable
                    {
                        LabOrderTypeId = request.LabOrderTypeId,
                        Name = dto.Name,
                        Unit = dto.Unit,
                        MaleMin = dto.MaleMin,
                        MaleMax = dto.MaleMax,
                        FemaleMin = dto.FemaleMin,
                        FemaleMax = dto.FemaleMax,
                        HasGenderRange = dto.HasGenderRange,
                        DisplayOrder = dto.DisplayOrder,
                        ResultType = dto.ResultType,
                        IsActive = true,
                        IsDelete = false,
                        CreatedById = userId,
                        CreatedDate = DateTime.Now,

                        LabTestVariableOptions = dto.Options?.Select(o => new Entities.Models.LabTestVariableOption
                        {
                            Name = o.Name,
                            DisplayOrder = o.DisplayOrder,
                            CreatedById = userId,
                            CreatedDate = DateTime.Now,
                            IsActive = true,
                            IsDelete = false
                        }).ToList()
                    };

                    await repo.AddAsync(entity);
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return 200;
        }


        private void UpdateOptions(Entities.Models.LabTestVariable entity, LabTestVariableDto dto)
        {
            var optionRepo = unitOfWork.Repository<LabTestVariableOption>();
            var existingOptionList = optionRepo
                .FindAllAsync(x => x.LabTestVariableId == entity.Id && x.IsActive && !x.IsDelete)
                .GetAwaiter()
                .GetResult()
                .ToList();

            var existingOptions = existingOptionList.ToDictionary(x => x.Id);

            var requestOptionIds = dto.Options?.Where(x => x.Id > 0).Select(x => x.Id).ToHashSet()
                                    ?? new HashSet<long>();

            // delete removed
            var toDelete = existingOptionList
                .Where(x => !requestOptionIds.Contains(x.Id))
                .ToList();

            foreach (var opt in toDelete)
            {
                opt.IsActive = false;
                opt.IsDelete = true;
                opt.ModifiedById = sessionProvider.Session.LoggedInUserId;
                opt.ModifiedDate = DateTime.Now;
                optionRepo.Update(opt);
            }

            // add/update
            foreach (var optDto in dto.Options ?? new List<LabTestVariableOptionDto>())
            {
                if (optDto.Id > 0 && existingOptions.TryGetValue(optDto.Id, out var existing))
                {
                    existing.Name = optDto.Name;
                    existing.DisplayOrder = optDto.DisplayOrder;
                    existing.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    existing.ModifiedDate = DateTime.Now;
                    optionRepo.Update(existing);
                }
                else
                {
                    optionRepo.Add(new LabTestVariableOption
                    {
                        LabTestVariableId = entity.Id,
                        Name = optDto.Name,
                        DisplayOrder = optDto.DisplayOrder,
                        IsActive = true,
                        IsDelete = false,
                        CreatedById = sessionProvider.Session.LoggedInUserId,
                        CreatedDate = DateTime.Now
                    });
                }
            }
        }
    }
}
