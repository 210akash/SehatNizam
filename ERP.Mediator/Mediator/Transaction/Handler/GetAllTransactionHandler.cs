using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Handler
{
    public class GetAllTransactionHandler : IRequestHandler<GetAllTransactionQuery, Tuple<IEnumerable<GetTransaction>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllTransactionHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetTransaction>, long>> Handle(GetAllTransactionQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Transaction, bool>> predicate;

            Expression<Func<Entities.Models.Transaction, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Status,
                x => x.Company,
                x => x.CreatedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.VoucherType,
                x => x.TransactionDocuments,
                x => x.TransactionDetails.Where(y => y.IsActive == true)  // Apply IsActive filter to the include
             };

            List<string> thenIncludes = new();
            thenIncludes.Add("TransactionDetails.Account");
            thenIncludes.Add("TransactionDetails.Account.AccountType");
            thenIncludes.Add("TransactionDetails.Department");
            thenIncludes.Add("TransactionDetails.Project");

            var fDate = request.FDate.Value.Date;  // Remove time component for start date
            var tDate = request.TDate.Value.Date.AddDays(1).AddTicks(-1);  // Set the end date to the last moment of the day


            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.VoucherTypeId == request.VoucherTypeId
                      && x.StatusId == request.StatusId
                      && x.Date >= fDate
                      && x.Date <= tDate
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if(roles.Contains("Accounts Assistant"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.VoucherTypeId == request.VoucherTypeId
                      && x.StatusId == request.StatusId
                      && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                       && x.Date >= fDate
                      && x.Date <= tDate
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.Date >= fDate
                      && x.Date <= tDate
                       && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.Transaction, object>> OrderBy = null;
            Expression<Func<Entities.Models.Transaction, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Transaction>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
            var Transaction = mapper.Map<IEnumerable<GetTransaction>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetTransaction>, long>(Transaction, entity.Item2);
        }
    }
}
