using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Interview.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Interview.Handler
{
    public class GetCodeHandler : IRequestHandler<GetCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCodeHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<string> Handle(GetCodeQuery request, CancellationToken cancellationToken)
        {
            string interviewCode = "";
            if (await unitOfWork.Repository<Entities.Models.Interview>().GetExistsAsync(x => x.IsActive && !x.IsDelete))
            {
                Func<IQueryable<Entities.Models.Interview>, IOrderedQueryable<Entities.Models.Interview>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var InterviewCode = await unitOfWork.Repository<Entities.Models.Interview>().GetOneAsync(y => y.IsActive == true
                , OrderByDesc, null);
                int No = Convert.ToInt32(InterviewCode.Code) + 1;
                interviewCode = No.ToString().PadLeft(7, '0');
            }
            else
                interviewCode = "0000001";

            return interviewCode;
        }
    }
}
