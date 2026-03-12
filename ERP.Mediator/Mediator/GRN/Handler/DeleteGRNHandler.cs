using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class DeleteGRNHandler : IRequestHandler<DeleteGRNQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteGRNHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        //public async Task<bool> Handle(DeleteGRNQuery request, CancellationToken cancellationToken)
        //{
        //    var GRN = await unitOfWork.Repository<Entities.Models.GRN>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
        //    GRN.IsDelete = true;
        //    GRN.IsActive = false;
        //    GRN.DeleteDate = DateTime.Now;
        //    GRN.ModifiedDate = DateTime.Now;
        //    GRN.ModifiedById = sessionProvider.Session.LoggedInUserId;
        //    unitOfWork.Repository<Entities.Models.GRN>().Update(GRN);
        //    await unitOfWork.SaveChangesAsync();
        //    return true;
        //}

        public async Task<bool> Handle(DeleteGRNQuery request, CancellationToken ct)
        {
            // 1️  Grab the GRN *with* its details and keep it tracked
            var grn = await unitOfWork.Repository<Entities.Models.GRN>().GetFirstAsync(y => y.Id == request.Id,null,null, "GRNDetails");

            if (grn is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            // 2️  Update the GRN itself
            grn.IsDelete = true;
            grn.IsActive = false;
            grn.DeleteDate = now;
            grn.ModifiedDate = now;
            grn.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in grn.GRNDetail)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            // 4️  Save once; EF writes one UPDATE for GRN + N UPDATEs for its details
            await unitOfWork.SaveChangesAsync(ct);
            return true;
        }
    }
}
