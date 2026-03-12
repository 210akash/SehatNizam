using MediatR;

namespace ERP.Mediator.Mediator.City.Query
{
    public class DeleteCityQuery : IRequest<bool>
    {
        public DeleteCityQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}