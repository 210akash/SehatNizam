using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Entities.Command
{
    public class StockTransactionDTO
    {
        public int ItemId { get; set; }
        public decimal StockQty { get; set; }
    }
}
