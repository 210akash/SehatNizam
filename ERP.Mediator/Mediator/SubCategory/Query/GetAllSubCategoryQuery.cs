using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Query
{
    public class GetAllSubCategoryQuery : IRequest<Tuple<IEnumerable<GetSubCategory>, long>>
    {
        public long? CategoryId { get; set; }
        public long? CompanyId { get; set; }
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}