using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeDocumentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Handler
{
    public class GetAllEmployeeDocumentTypeHandler : IRequestHandler<GetAllEmployeeDocumentTypeQuery, Tuple<IEnumerable<GetEmployeeDocumentType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeDocumentTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeDocumentType>, long>> Handle(GetAllEmployeeDocumentTypeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeDocumentType, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.EmployeeDocumentType, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.EmployeeDocumentType, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeDocumentType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeDocumentType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var employeeDocumentType = mapper.Map<IEnumerable<GetEmployeeDocumentType>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetEmployeeDocumentType>, long>(employeeDocumentType, entity.Item2);
        }
    }
}
