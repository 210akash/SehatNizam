using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IPD.AdmissionPackage.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionPackage.Handler
{
    public class GetAllAdmissionPackageMasterHandler : IRequestHandler<GetAllAdmissionPackageMasterQuery, Tuple<IEnumerable<GetAdmissionPackageMaster>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllAdmissionPackageMasterHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetAdmissionPackageMaster>, long>> Handle(GetAllAdmissionPackageMasterQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.AdmissionPackageMaster, bool>> predicate = x => x.IsActive == true
                && (request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower().Trim()));

            Expression<Func<Entities.Models.AdmissionPackageMaster, object>>[] includes = {
                x => x.AdmissionPackageDetail.Where(d => d.IsActive == true)
            };

            var thenIncludes = new List<string> { "AdmissionPackageDetail.Service" };

            Expression<Func<Entities.Models.AdmissionPackageMaster, object>> OrderBy = null;
            Expression<Func<Entities.Models.AdmissionPackageMaster, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.AdmissionPackageMaster>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var packages = mapper.Map<IEnumerable<GetAdmissionPackageMaster>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetAdmissionPackageMaster>, long>(packages, entity.Item2);
        }
    }
}
