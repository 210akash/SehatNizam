using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Category.Query
{
    public class GetCategoryCodeQuery : IRequest<string>
    {
    }
}