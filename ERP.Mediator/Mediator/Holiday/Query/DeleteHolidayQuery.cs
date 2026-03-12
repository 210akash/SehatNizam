using MediatR;

namespace ERP.Mediator.Mediator.Holiday.Query
{
    public class DeleteHolidayQuery : IRequest<bool>
    {
        public DeleteHolidayQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}