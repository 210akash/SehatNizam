using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.PrimaryOrder.Command
{
    public class SaveDistributorPricingGroupCommand : IRequest<long>
    {
        public long GroupId { get; set; }
        public List<GetAllDistributorByGroupId> GetAllDistributorByGroupId { get; set; }
    }
}
