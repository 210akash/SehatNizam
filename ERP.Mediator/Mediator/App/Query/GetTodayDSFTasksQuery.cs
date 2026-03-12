using ERP.BusinessModels.ResponseVM.AppVM;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.App.Query
{
    public class GetTodayDSFTasksQuery : IRequest<Tuple<List<GetTodayDSFTasks>, long>>
    {
        public GetTodayDSFTasksQuery(string DSFId, DateTime AppDateTime)
        {
            this.DSFId = DSFId;
            this.AppDateTime = AppDateTime;
        }

        public string DSFId { get; set; }
        public DateTime AppDateTime { get; set; }
    }
}