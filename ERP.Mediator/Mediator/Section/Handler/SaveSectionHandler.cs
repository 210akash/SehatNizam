using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Section.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Section.Handler
{
    public class SaveSectionHandler : IRequestHandler<SaveSectionCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveSectionHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveSectionCommand, long>.Handle(SaveSectionCommand request, CancellationToken cancellationToken)
        {
            var Section = await unitOfWork.Repository<Entities.Models.Section>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            //var checkDuplicate = await unitOfWork.Repository<Entities.Models.Section>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            //if (checkDuplicate.Count() == 0)
            //{
            if (Section == null)
            {
                var _Section = mapper.Map<Entities.Models.Section>(request);
                _Section.CreatedById = sessionProvider.Session.LoggedInUserId;
                _Section.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Section>().Add(_Section);
                SaveChanges();
            }
            else
            {
                var _Section = mapper.Map<Entities.Models.Section>(request);
                _Section.CreatedById = Section.CreatedById;
                _Section.CreatedDate = Section.CreatedDate;
                _Section.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _Section.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Section>().Update(_Section);
                SaveChanges();
            }
            return 200;
        }
    }
}