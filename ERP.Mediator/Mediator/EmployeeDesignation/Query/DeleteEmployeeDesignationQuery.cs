using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDesignation.Query
{
    public class DeleteEmployeeDesignationQuery : IRequest<bool>
    {
        public DeleteEmployeeDesignationQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}