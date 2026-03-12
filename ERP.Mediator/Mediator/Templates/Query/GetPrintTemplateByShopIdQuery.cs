using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Templates.Query
{
    public class GetPrintTemplateByShopIdQuery : IRequest<string>
    {
        public GetPrintTemplateByShopIdQuery(long ShopId, long TemplateId)
        {
            this.ShopId = ShopId;
            this.TemplateId = TemplateId;
        }

        public long ShopId { get; set; }
        public long TemplateId { get; set; }
    }
}
