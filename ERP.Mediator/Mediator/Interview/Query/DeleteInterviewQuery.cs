using MediatR;

namespace ERP.Mediator.Mediator.Interview.Query
{
    public class DeleteInterviewQuery : IRequest<bool>
    {
        public DeleteInterviewQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}