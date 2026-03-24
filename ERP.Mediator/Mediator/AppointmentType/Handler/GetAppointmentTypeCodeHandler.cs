using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.AppointmentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.AppointmentType.Handler
{
    public class GetAppointmentTypeCodeHandler : IRequestHandler<GetAppointmentTypeCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetAppointmentTypeCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetAppointmentTypeCodeQuery request, CancellationToken cancellationToken)
        {
            string _AppointmentTypeCode = "";
            if (await unitOfWork.Repository<Entities.Models.AppointmentType>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.IsActive == true))
            {
                Func<IQueryable<Entities.Models.AppointmentType>, IOrderedQueryable<Entities.Models.AppointmentType>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var AppointmentTypeCode = await unitOfWork.Repository<Entities.Models.AppointmentType>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                int No = Convert.ToInt32(AppointmentTypeCode.Code) + 1;
                _AppointmentTypeCode = No.ToString().PadLeft(2, '0');
            }
            else
                _AppointmentTypeCode = "01";
          
            return _AppointmentTypeCode;
        }
    }
}
