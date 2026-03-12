using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class UpdateSelectedWarehouseQuery : IRequest<string>
    {
        public UpdateSelectedWarehouseQuery(long Projectid)
        {
            this.Projectid = Projectid;
        }

        public long Projectid { get; set; }
    }
}
