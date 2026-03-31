using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PrimaryOrder.Query
{
    public class GetAllAppointmentStatusQuery : IRequest<List<GetAppointmentStatus>>
    {

    }
}