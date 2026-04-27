using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Payroll.EmployeeSalary.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.City.Handler
{
    public class SaveEmployeeSalaryHandler : IRequestHandler<SaveEmployeeSalaryCommand, Tuple<long, string>>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeSalaryHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<Tuple<long, string>> Handle(SaveEmployeeSalaryCommand request, CancellationToken cancellationToken)
        {
            var repo = unitOfWork.Repository<Entities.Models.EmployeeSalary>();

            // Get existing records for employee
            var existingSalaries = await repo
                .GetAsync(x => x.EmployeeId == request.EmployeeId && x.IsActive == true);

            var existingList = existingSalaries.ToList();

            // 🔴 Validation: Duplicate SalaryHead in request
            //var duplicateHeads = request.EmployeeSalary
            //    .GroupBy(x => x.SalaryHeadId)
            //    .Where(g => g.Count() > 1)
            //    .Select(g => g.Key)
            //    .ToList();

            //if (duplicateHeads.Any())
            //{
            //    return new Tuple<long, string>(409, "Duplicate Salary Head found in request.");
            //}

            // 🔴 Delete removed SalaryHeads
            var requestIds = request.EmployeeSalary.Select(x => x.Id).ToList();
            var toDelete = existingList.Where(x => !requestIds.Contains(x.Id)).ToList();

            foreach (var item in toDelete)
            {
                item.IsActive = false;
                item.IsDelete = true;
                item.DeleteDate = DateTime.Now;
                item.ModifiedById = sessionProvider.Session.LoggedInUserId;

                repo.Update(item);
            }

            // 🔵 Insert / Update
            foreach (var item in request.EmployeeSalary)
            {
                if (item.Id != 0)
                {
                    // Update
                    var existing = existingList.FirstOrDefault(x => x.Id == item.Id);
                    if (existing != null)
                    {
                        existing.SalaryHeadId = item.SalaryHeadId;
                        existing.Amount = item.Amount;
                        existing.EffectiveFrom = item.EffectiveFrom;

                        existing.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        existing.ModifiedDate = DateTime.Now;

                        repo.Update(existing);
                    }
                }
                else
                {
                    // Check if same SalaryHead already exists (avoid duplicate insert)
                    var alreadyExists = existingList
                        .FirstOrDefault(x => x.SalaryHeadId == item.SalaryHeadId && x.IsActive);

                    if (alreadyExists != null)
                    {
                        // Update instead of insert
                        alreadyExists.Amount = item.Amount;
                        alreadyExists.EffectiveFrom = item.EffectiveFrom;

                        alreadyExists.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        alreadyExists.ModifiedDate = DateTime.Now;

                        repo.Update(alreadyExists);
                    }
                    else
                    {
                        // Insert new
                        var entity = new Entities.Models.EmployeeSalary
                        {
                            EmployeeId = request.EmployeeId,
                            SalaryHeadId = item.SalaryHeadId,
                            Amount = item.Amount,
                            EffectiveFrom = item.EffectiveFrom,

                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now
                        };

                        repo.Add(entity);
                    }
                }
            }

            var result = SaveChanges();

            if (result > 0)
                return new Tuple<long, string>(200, "Saved Successfully");

            return new Tuple<long, string>(500, "Error while saving");
        }
    }
}