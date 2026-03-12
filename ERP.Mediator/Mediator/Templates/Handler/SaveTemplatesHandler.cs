using AutoMapper;
using MediatR;
using ERP.Repositories.UnitOfWork;
using System.Threading;
using System.Threading.Tasks;
using System;
using ERP.Mediator.Mediator.Templates.Command;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.Templates.Handler
{
    public class SaveTemplatesHandler : IRequestHandler<SaveTemplatesCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveTemplatesHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveTemplatesCommand, long>.Handle(SaveTemplatesCommand request, CancellationToken cancellationToken)
        {
            var templates = await unitOfWork.Repository<ERP.Entities.Models.Templates>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (templates == null)
            {
                var _templates = mapper.Map< ERP.Entities.Models.Templates>(request);
                _templates.CreatedById = sessionProvider.Session.LoggedInUserId;
                _templates.CreatedDate = DateTime.Now;
                unitOfWork.Repository<ERP.Entities.Models.Templates>().Add(_templates);
                SaveChanges();
            }
            else
            {
                var _templates = mapper.Map<ERP.Entities.Models.Templates>(request);
                _templates.CreatedById = templates.CreatedById;
                _templates.CreatedDate = templates.CreatedDate;
                _templates.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _templates.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<ERP.Entities.Models.Templates>().Update(_templates);
                SaveChanges();
            }
            return 200;
        }
    }
}
