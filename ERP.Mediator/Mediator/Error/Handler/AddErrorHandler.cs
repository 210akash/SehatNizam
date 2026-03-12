using AutoMapper;
using Microsoft.Extensions.Configuration;
using MediatR;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Repositories.UnitOfWork;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.Error.Command;

namespace ERP.Mediator.Mediator.Error.Handler
{
    public class AddErrorHandler : BaseHandler, IRequestHandler<AddErrorCommand, bool>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly ERPDbContext _context;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        public AddErrorHandler(IMapper mapper, IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper, ERPDbContext context, IConfiguration config, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.unitOfWorkDapper = unitOfWorkDapper;
            _context = context;
        }


        public async Task<bool> Handle(AddErrorCommand request, CancellationToken cancellationToken)
        {
            var model = new ErrorLogs()
            {
                Message = request.Message,
                StackTrace = request.StackTrace,
                UserId = request.UserId,
                Created = DateTime.UtcNow
            };

            await this.unitOfWork.Repository<ErrorLogs>().AddAsync(model);
            await this.unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}

