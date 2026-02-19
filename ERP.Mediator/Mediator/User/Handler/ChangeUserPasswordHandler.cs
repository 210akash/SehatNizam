using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.User.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.User.Handler
{
    public class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPasswordCommand, bool>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ChangeUserPasswordHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }
        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }
        async Task<bool> IRequestHandler<ChangeUserPasswordCommand, bool>.Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            return false;
        }
    }
}
