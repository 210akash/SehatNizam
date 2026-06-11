using System;

using System.Collections.Generic;

using ERP.BusinessModels.ResponseVM;

using ERP.Entities.Models;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Donor.Query

{

    public class GetAllBloodDonorQuery : IRequest<Tuple<IEnumerable<GetBloodDonor>, long>>

    {

        public string Name { get; set; }

        public string CNIC { get; set; }

        public PagingData PagingData { get; set; }

    }

}

