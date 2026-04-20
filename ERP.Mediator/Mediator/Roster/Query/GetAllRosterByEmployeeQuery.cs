using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Roster.Query
{
    public class GetAllRosterByEmployeeQuery : IRequest<GetRoster>
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
