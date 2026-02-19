using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Category.Query
{
    public class GetCategoryByIdQuery : IRequest<GetCategory>
    {
        public GetCategoryByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}