using System;

using System.Collections.Generic;

using ERP.BusinessModels.ResponseVM;

using ERP.Entities.Models;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Fridge.Query

{

    public class GetAllFridgeQuery : IRequest<Tuple<IEnumerable<GetBloodFridge>, long>>

    {

        public string Name { get; set; }

        public PagingData PagingData { get; set; }

    }

}

