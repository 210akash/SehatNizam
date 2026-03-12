using MediatR;

namespace ERP.Mediator.Mediator.EmployeeWorkSiteType.Query
{
    public class DeleteEmployeeWorkSiteTypeQuery : IRequest<bool>
    {
        public DeleteEmployeeWorkSiteTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}