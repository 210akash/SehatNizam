using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.City.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.City.Handler
{
    public class DeleteCityHandler : IRequestHandler<DeleteCityQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteCityHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteCityQuery request, CancellationToken cancellationToken)
        {
            var city = await unitOfWork.Repository<Entities.Models.City>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            city.IsDelete = true;
            city.IsActive = false;
            city.DeleteDate = DateTime.Now;
            city.ModifiedDate = DateTime.Now;
            city.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.City>().Update(city);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
