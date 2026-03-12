using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.App.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;
using Dapper;
using System.Data;
using ERP.Entities.Models;

namespace ERP.Mediator.Mediator.App.Handler
{
    public class GetDealershipAccountLedgerHandler : IRequestHandler<GetDealershipAccountLedgerQuery, IEnumerable<GetDealershipAccountLedger>>
    {
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        private readonly IUnitOfWork unitOfWork;
        public GetDealershipAccountLedgerHandler(IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper)
        {
            this.unitOfWork = unitOfWork;
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<IEnumerable<GetDealershipAccountLedger>> Handle(GetDealershipAccountLedgerQuery request, CancellationToken cancellationToken)
        {
            var accountGroup = await unitOfWork
                .Repository<ERP.Entities.Models.AccountGroup>()
                .GetFirstAsNoTrackingAsync(y => y.DealershipId == request.DealershipId);

            if (accountGroup != null)
            {
                var query = "AccountLedgerSP";
                var parameters = new DynamicParameters();

                parameters.Add("@FromDate", request.FDate, DbType.DateTime);
                parameters.Add("@ToDate", request.TDate, DbType.DateTime);
                parameters.Add("@Account", accountGroup.Code, DbType.String);

                // ✅ Corrected type to match stored procedure
                parameters.Add("@VoucherTypeId", DBNull.Value, DbType.String);

                var regionResponse = (await unitOfWorkDapper
                    .Repository<GetDealershipAccountLedger>()
                    .QueryAsync<GetDealershipAccountLedger>(
                        query,
                        parameters,
                        CommandType.StoredProcedure)).ToList();

                return regionResponse;
            }

            return null;
        }
    }
}
