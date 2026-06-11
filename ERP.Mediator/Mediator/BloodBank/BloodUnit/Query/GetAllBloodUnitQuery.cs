using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.BloodUnit.Query
{
    public class GetAllBloodUnitQuery : IRequest<Tuple<IEnumerable<GetBloodUnit>, long>>
    {
        public string UnitNo { get; set; }
        public string ComponentTypeName { get; set; }
        public int? Status { get; set; }

        /// <summary>0 = all, 1 = assigned, 2 = not assigned</summary>
        public int? StorageAssigned { get; set; }

        public PagingData PagingData { get; set; }
    }
}
