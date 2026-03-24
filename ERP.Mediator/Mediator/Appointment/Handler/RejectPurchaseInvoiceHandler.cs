using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class RejectPurchaseInvoiceHandler : IRequestHandler<RejectPurchaseInvoiceQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMapper mapper;
        private readonly List<string> codes = new();

        public RejectPurchaseInvoiceHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;
        }

        public async Task<long> Handle(RejectPurchaseInvoiceQuery request, CancellationToken cancellationToken)
        {
            var GRNUpdate = await unitOfWork.Repository<Entities.Models.GRN>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);

            if(GRNUpdate != null)
            {
                if(GRNUpdate.InvoiceStatusId == 6)
                {
                    GRNUpdate.InvoiceStatusId = 2;
                }
                else if (GRNUpdate.InvoiceStatusId == 2)
                {
                    GRNUpdate.InvoiceStatusId = 1;
                }

                GRNUpdate.Comments = (GRNUpdate.Comments ?? "")
                     + (string.IsNullOrWhiteSpace(GRNUpdate.Comments) ? "" : Environment.NewLine)
                     + request.Comments;

                GRNUpdate.ModifiedDate = DateTime.Now;
                GRNUpdate.ModifiedById = sessionProvider.Session.LoggedInUserId;
            }
            
            unitOfWork.Repository<Entities.Models.GRN>().Update(GRNUpdate);
            var check = await unitOfWork.SaveChangesAsync();

            if (check > 0)
            {
                return 200;
            }
            else
            {
                return 500;
            }
        }


    }
}
