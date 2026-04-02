using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Patient.Query
{
    public class GetPatientByNameQuery : IRequest<List<GetPatient>>
    {
        public GetPatientByNameQuery(string Search)
        {
            this.Search = Search;
        }

        public string Search { get; set; }
    }
}