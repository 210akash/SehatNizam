using System;

using System.Collections.Generic;

using ERP.BusinessModels.ResponseVM;

using ERP.Entities.Models;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Donation.Query

{

    public class GetAllBloodDonationQuery : IRequest<Tuple<IEnumerable<GetBloodDonation>, long>>

    {

        public long? BloodDonorId { get; set; }

        public string DonorName { get; set; }

        public string DonorCNIC { get; set; }

        public int? ScreeningStatus { get; set; }

        public long? AppointmentId { get; set; }

        public PagingData PagingData { get; set; }

    }

}

