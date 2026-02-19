using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.ComplexTypes;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Auth.Query;
//using CRM.Mediator.Mediator.CRM.Query;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class GetAllRolesHandler : IRequestHandler<GetAspNetRolesQuery, List<GetRoles>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetAllRolesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRoles>> Handle(GetAspNetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await unitOfWork.Repository<AspNetRoles>().GetAllAsync();
            return mapper.Map<List<GetRoles>>(roles);
        }
    }
}
