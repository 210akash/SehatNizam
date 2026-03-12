using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Command
{
    public class SaveItemTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long SubCategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
