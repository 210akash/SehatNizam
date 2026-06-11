using System;

using System.Collections.Generic;

using ERP.BusinessModels.ResponseVM;

using ERP.Entities.Models;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Rack.Query

{

    public class GetAllRackQuery : IRequest<Tuple<IEnumerable<GetBloodRack>, long>>

    {

        public string Name { get; set; }

        public long? BloodFridgeId { get; set; }

        public PagingData PagingData { get; set; }

    }

}

