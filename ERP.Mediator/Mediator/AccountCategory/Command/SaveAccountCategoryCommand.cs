using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.AccountCategory.Command
{
    public class SaveAccountCategoryCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public long AccountHeadId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
