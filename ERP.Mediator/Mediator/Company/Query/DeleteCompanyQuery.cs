using MediatR;

namespace ERP.Mediator.Mediator.Company.Query
{
    public class DeleteCompanyQuery : IRequest<bool>
    {
        public DeleteCompanyQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}