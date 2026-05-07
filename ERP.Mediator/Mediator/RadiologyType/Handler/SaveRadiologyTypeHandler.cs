using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RadiologyType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyType.Handler
{
    public class SaveRadiologyTypeHandler : IRequestHandler<SaveRadiologyTypeCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public SaveRadiologyTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SaveRadiologyTypeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return 400;
            }

            Entities.Models.RadiologyType radiologyType;

            if (request.Id > 0)
            {
                radiologyType = await unitOfWork.Repository<Entities.Models.RadiologyType>().FindAsync(y => y.Id == request.Id);
                if (radiologyType == null)
                {
                    return 404;
                }

                mapper.Map(request, radiologyType);
                radiologyType.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
                radiologyType.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.RadiologyType>().Update(radiologyType);
            }
            else
            {
                var exists = await unitOfWork.Repository<Entities.Models.RadiologyType>()
                    .GetExistsAsync(x => x.Name.ToLower() == request.Name.ToLower()
                        && x.IsActive
                        && !x.IsDelete
                        && x.CompanyId == this.sessionProvider.Session.CompanyId);

                if (exists)
                {
                    return 409;
                }

                radiologyType = mapper.Map<Entities.Models.RadiologyType>(request);
                radiologyType.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                radiologyType.CompanyId = this.sessionProvider.Session.CompanyId;
                radiologyType.IsActive = true;

                await unitOfWork.Repository<Entities.Models.RadiologyType>().AddAsync(radiologyType);
            }

            await unitOfWork.SaveChangesAsync();
            return 200;
        }
    }
}


