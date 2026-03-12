using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ComparativeStatement.Command
{
    public class SaveComparativeStatementCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long PurchaseDemandId { get; set; }
        public long StatusId { get; set; }

        public string Remarks { get; set; }
        public virtual List<SaveComparativeStatementDetailCommand> ComparativeStatementDetail { get; set; }
    }

    public class SaveComparativeStatementDetailCommand
    {
        public long Id { get; set; }
        public long ComparativeStatementId { get; set; }
        public long PurchaseDemandDetailId { get; set; }
        public virtual List<SaveComparativeStatementVendorCommand> ComparativeStatementVendor { get; set; }
    }

    public class SaveComparativeStatementVendorCommand
    {
        public long Id { get; set; }
        public long ComparativeStatementDetailId { get; set; }
        public long VendorId { get; set; }
        public decimal Price { get; set; }
    }
}
