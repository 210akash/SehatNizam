using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Command
{
    public class SaveSubCategoryCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
