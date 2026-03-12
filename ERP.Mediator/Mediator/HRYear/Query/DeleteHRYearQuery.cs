using MediatR;

namespace ERP.Mediator.Mediator.HRYear.Query
{
    public class DeleteHRYearQuery : IRequest<bool>
    {
        public DeleteHRYearQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}