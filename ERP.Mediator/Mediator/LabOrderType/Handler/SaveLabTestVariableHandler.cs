using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.LabOrderType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

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
