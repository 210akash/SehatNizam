using MediatR;

namespace ERP.Mediator.Mediator.AccountType.Command
{
    public class SaveAccountTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AccountSubCategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
