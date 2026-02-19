using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Command
{
    public class SaveAccountSubCategoryCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AccountCategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
