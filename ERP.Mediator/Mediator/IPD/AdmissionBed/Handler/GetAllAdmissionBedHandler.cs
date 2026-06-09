using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IPD.AdmissionBed.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionBed.Handler
{
    public class GetAllAdmissionBedHandler : IRequestHandler<GetAllAdmissionBedQuery, Tuple<IEnumerable<GetAdmissionBed>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllAdmissionBedHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetAdmissionBed>, long>> Handle(GetAllAdmissionBedQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.AdmissionBed, bool>> predicate = x => x.IsActive == true && x.IsDelete == false
            && x.AdmissionId == request.AdmissionId;

            Expression<Func<Entities.Models.AdmissionBed, object>>[] includes = {
                x => x.Bed,
                x => x.Bed.Room,
                x => x.Bed.Room.Ward
            };

            Expression<Func<Entities.Models.AdmissionBed, object>> OrderBy = null;
            Expression<Func<Entities.Models.AdmissionBed, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.AdmissionBed>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var Project = mapper.Map<IEnumerable<GetAdmissionBed>>(entity.Item1.ToList());
            return new Tuple<IEnumerable<GetAdmissionBed>, long>(Project, entity.Item2);
        }
    }
}
