using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.UserAttendance.Query
{
    public class GetUserAttendanceByNameQuery : IRequest<List<GetUserAttendance>>
    {
        public GetUserAttendanceByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}