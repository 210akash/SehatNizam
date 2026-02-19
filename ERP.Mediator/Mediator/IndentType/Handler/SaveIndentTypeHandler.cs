using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.IndentType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Handler
{
    public class SaveIndentTypeHandler : IRequestHandler<SaveIndentTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveIndentTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveIndentTypeCommand, long>.Handle(SaveIndentTypeCommand request, CancellationToken cancellationToken)
        {
            var IndentType = await unitOfWork.Repository<Entities.Models.IndentType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.IndentType>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (IndentType == null)
                {
                    var _IndentType = mapper.Map<Entities.Models.IndentType>(request);
                    _IndentType.CompanyId = sessionProvider.Session.CompanyId;
                    _IndentType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _IndentType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.IndentType>().Add(_IndentType);
                    SaveChanges();
                }
                else
                {
                    var _IndentType = mapper.Map<Entities.Models.IndentType>(request);
                    _IndentType.CompanyId = IndentType.CompanyId;
                    _IndentType.CreatedById = IndentType.CreatedById;
                    _IndentType.CreatedDate = IndentType.CreatedDate;
                    _IndentType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _IndentType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.IndentType>().Update(_IndentType);
                    SaveChanges();
                }
                return 200;

            }
            else
            {
                return 409;
            }

        }
    }
}