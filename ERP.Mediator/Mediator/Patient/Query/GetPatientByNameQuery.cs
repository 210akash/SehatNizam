using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Patient.Query
{
    public class GetPatientByNameQuery : IRequest<List<GetPatient>>
    {
        public GetPatientByNameQuery(string search)
        {
            this.search = search;
        }

        public string search { get; set; }
    }
}