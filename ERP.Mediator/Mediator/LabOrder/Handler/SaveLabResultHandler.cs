using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.LabOrder.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.LabOrder.Handler
{
    public class SaveLabResultHandler : IRequestHandler<SaveLabResultCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public SaveLabResultHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveLabResultCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.Results)
            {
                var variable = await unitOfWork
                    .Repository<LabTestVariable>().GetFirstAsNoTrackingAsync(x => x.Id == item.LabTestVariableId);

                if (variable == null)
                    continue;

                string referenceRange = "";

                if (variable.HasGenderRange)
                {
                    referenceRange =
                        $"Male: {variable.MaleMin}-{variable.MaleMax}, " +
                        $"Female: {variable.FemaleMin}-{variable.FemaleMax}";
                }
                else
                {
                    referenceRange =
                        $"{variable.MaleMin}-{variable.MaleMax}";
                }

                var result = new LabResult
                {
                    LabOrderId = request.LabOrderId,
                    LabTestVariableId = variable.Id,
                    ResultValue = item.ResultValue,
                    // Snapshot values
                    VariableName = variable.Name,
                    Unit = variable.Unit,
                    ReferenceRange = referenceRange,
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now,
                    IsAbnormal = null
                };

                unitOfWork.Repository<LabResult>().Add(result);
            }

            await unitOfWork.SaveChangesAsync();
            return 200;
        }
    }
}
