using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.IPD.AdmissionPackage.Query
{
    public class GetAllAdmissionPackageMasterQuery : IRequest<Tuple<IEnumerable<GetAdmissionPackageMaster>, long>>
    {
        public string Name { get; set; }
        public PagingData PagingData { get; set; }
    }
}
