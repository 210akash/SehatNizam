using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Row.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Row.Handler
{
    public class SaveRowHandler : IRequestHandler<SaveRowCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRowHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveRowCommand, long>.Handle(SaveRowCommand request, CancellationToken cancellationToken)
        {
            var Row = await unitOfWork.Repository<Entities.Models.Row>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            //var checkDuplicate = await unitOfWork.Repository<Entities.Models.Row>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            //if (checkDuplicate.Count() == 0)
            //{
            if (Row == null)
            {
                var _Row = mapper.Map<Entities.Models.Row>(request);
                _Row.CreatedById = sessionProvider.Session.LoggedInUserId;
                _Row.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Row>().Add(_Row);
                SaveChanges();
            }
            else
            {
                var _Row = mapper.Map<Entities.Models.Row>(request);
                _Row.CreatedById = Row.CreatedById;
                _Row.CreatedDate = Row.CreatedDate;
                _Row.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _Row.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Row>().Update(_Row);
                SaveChanges();
            }
            return 200;
        }
    }
}