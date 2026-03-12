using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Interview.Query
{
    public class GetInterviewAttendeesQuery : IRequest<List<GetAllUsers>>
    {
    }
}