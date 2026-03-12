using MediatR;
using ERP.BusinessModels.ParameterVM;
using System;
using System.Collections.Generic;
using ERP.Entities.Models;

namespace ERP.Mediator.Mediator.Templates.Query
{
    public class GetAllTemplatesQuery : IRequest<Tuple<IEnumerable<GetTemplates>, long>>
    {
        public PagingData PagingData { get; set; }
    }
}
