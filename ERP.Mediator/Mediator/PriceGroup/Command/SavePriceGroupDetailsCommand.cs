using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.PriceGroup.Command
{
    public class SavePriceGroupDetailsCommand : IRequest<long>
    {
        public long Id { get; set; }
        public List<GetItemGroupDetails> GetProductGroupDetails { get; set; } = new List<GetItemGroupDetails>();
    }
}
