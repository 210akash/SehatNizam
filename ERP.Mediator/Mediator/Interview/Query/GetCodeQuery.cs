using MediatR;

namespace ERP.Mediator.Mediator.Interview.Query
{
    public class GetCodeQuery : IRequest<string>
    {
        public GetCodeQuery()
        {
        }
    }
}