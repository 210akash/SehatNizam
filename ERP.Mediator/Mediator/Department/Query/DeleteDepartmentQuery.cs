using MediatR;

namespace ERP.Mediator.Mediator.Department.Query
{
    public class DeleteDepartmentQuery : IRequest<bool>
    {
        public DeleteDepartmentQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}