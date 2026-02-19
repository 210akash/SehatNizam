using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.ComplexTypes;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class GetAllUsersQuery : IRequest<List<GetAllUsers>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllUsersQuery"/> class.
        /// </summary>
        /// <param name="id">Id.</param>
        public GetAllUsersQuery()
        {
            //Guid guid = Guid.NewGuid();
            //string id = new Guid(guid.ToString()).ToString();
            //this.UserName = UserName;
        }

        /// <summary>
        /// Gets or sets the Id of spGetUsers.
        /// </summary>
        /// <value>
        /// The Id of the spGetUsers.
        /// </value>
        public string UserName { get; set; }
    }
}