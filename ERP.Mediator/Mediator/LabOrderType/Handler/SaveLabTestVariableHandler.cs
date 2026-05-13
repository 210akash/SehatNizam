using ERP.Core.Provider;
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
            {
                return 400; // Bad request
            }

            // Get existing active variables for this LabOrderType
            var existingVariables = await unitOfWork.Repository<Entities.Models.LabTestVariable>()
                .FindAllAsync(x => x.LabOrderTypeId == request.LabOrderTypeId && x.IsActive && !x.IsDelete);

            List<long> existingVariableIds = existingVariables.Select(x => x.Id).ToList();
            List<long> currentVariableIds = request.Variables.Where(x => x.Id != 0).Select(x => x.Id).ToList();

            // Identify deleted variables
            var deletedVariableIds = existingVariableIds.Except(currentVariableIds).ToList();

            foreach (var deletedId in deletedVariableIds)
            {
                var variableToDelete = existingVariables.FirstOrDefault(x => x.Id == deletedId);
                if (variableToDelete != null)
                {
                    variableToDelete.IsActive = false;
                    variableToDelete.IsDelete = true;
                    variableToDelete.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
                    variableToDelete.DeleteDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.LabTestVariable>().Update(variableToDelete);
                }
            }

            // Add new or update existing variables
            foreach (var variableDto in request.Variables)
            {
                if (variableDto.Id != 0) // Update existing
                {
                    var existing = existingVariables.FirstOrDefault(x => x.Id == variableDto.Id);
                    if (existing != null)
                    {
                        existing.Name = variableDto.Name;
                        existing.Unit = variableDto.Unit;
                        existing.MaleMin = variableDto.MaleMin;
                        existing.MaleMax = variableDto.MaleMax;
                        existing.FemaleMin = variableDto.FemaleMin;
                        existing.FemaleMax = variableDto.FemaleMax;
                        existing.HasGenderRange = variableDto.HasGenderRange;
                        existing.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
                        existing.ModifiedDate = DateTime.Now;

                        unitOfWork.Repository<Entities.Models.LabTestVariable>().Update(existing);
                    }
                }
                else // Add new
                {
                    var newVariable = new Entities.Models.LabTestVariable
                    {
                        LabOrderTypeId = request.LabOrderTypeId,
                        Name = variableDto.Name,
                        Unit = variableDto.Unit,
                        MaleMin = variableDto.MaleMin,
                        MaleMax = variableDto.MaleMax,
                        FemaleMin = variableDto.FemaleMin,
                        FemaleMax = variableDto.FemaleMax,
                        HasGenderRange = variableDto.HasGenderRange,
                        CreatedById = this.sessionProvider.Session.LoggedInUserId,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        IsDelete = false
                    };

                    await unitOfWork.Repository<Entities.Models.LabTestVariable>().AddAsync(newVariable);
                }
            }

            await unitOfWork.SaveChangesAsync();
            return 200; // Success
        }

        public async Task<int> Handle1(SaveLabTestVariableCommand request, CancellationToken cancellationToken)
        {
            if (request.LabOrderTypeId <= 0 || request.Variables == null || !request.Variables.Any())
            {
                return 400;
            }

            // Delete existing variables for this LabOrderType (soft delete)
            var existingVariables = await unitOfWork.Repository<Entities.Models.LabTestVariable>()
                .FindAllAsync(x => x.LabOrderTypeId == request.LabOrderTypeId && x.IsActive && !x.IsDelete);

            foreach (var existing in existingVariables)
            {
                existing.IsDelete = true;
                existing.IsActive = false;
                existing.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
                existing.DeleteDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.LabTestVariable>().Update(existing);
            }

            // Add new variables
            foreach (var variableDto in request.Variables)
            {
                var variable = new Entities.Models.LabTestVariable
                {
                    LabOrderTypeId = request.LabOrderTypeId,
                    Name = variableDto.Name,
                    Unit = variableDto.Unit,
                    MaleMin = variableDto.MaleMin,
                    MaleMax = variableDto.MaleMax,
                    FemaleMin = variableDto.FemaleMin,
                    FemaleMax = variableDto.FemaleMax,
                    HasGenderRange = variableDto.HasGenderRange,
                    CreatedById = this.sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDelete = false
                };

                await unitOfWork.Repository<Entities.Models.LabTestVariable>().AddAsync(variable);
            }

            await unitOfWork.SaveChangesAsync();
            return 200;
        }
    }
}
