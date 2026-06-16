using System;

using System.Collections.Generic;

using ERP.BusinessModels.ResponseVM;

using ERP.Entities.Models;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Request.Query

{

    public class GetAllBloodRequestQuery : IRequest<Tuple<IEnumerable<GetBloodRequest>, long>>

    {

        public string PatientCNIC { get; set; }

        public string PatientName { get; set; }

        public long? BloodGroupMasterId { get; set; }

        public long? BloodComponentTypeId { get; set; }

        public long? AppointmentId { get; set; }

        public int? Status { get; set; }

        public PagingData PagingData { get; set; }

    }

}

