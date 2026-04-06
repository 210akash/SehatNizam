using MediatR;

namespace ERP.Mediator.Mediator.SugarType.Query
{
    public class DeleteSugarTypeQuery : IRequest<bool>
    {
        public DeleteSugarTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}