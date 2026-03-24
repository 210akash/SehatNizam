using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.GRN.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class SavePurchaseInvoiceWHTHandler : IRequestHandler<SavePurchaseInvoiceWHTCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;

        public SavePurchaseInvoiceWHTHandler(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SavePurchaseInvoiceWHTCommand, long>.Handle(SavePurchaseInvoiceWHTCommand request, CancellationToken cancellationToken)
        {
            var GRN = await unitOfWork.Repository<Entities.Models.GRN>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (GRN != null)
            {
                GRN.WHTPercentage = request.WHTPercentage;
                GRN.ModifiedById = sessionProvider.Session.LoggedInUserId;
                GRN.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.GRN>().Update(GRN);
                SaveChanges();
            }
            return 200;
        }
    }
}