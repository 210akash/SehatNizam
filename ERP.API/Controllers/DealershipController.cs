using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Dealership.Query;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.Dealership.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DealershipController : ControllerBase
    {
        private readonly IMediator mediator;

        public DealershipController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetDealershipById")]
        public async Task<ActionResult<GetDealership>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetDealershipByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllDealership")]
        public async Task<ActionResult<Tuple<IEnumerable<GetDealership>, long>>> GetAll(GetAllDealershipQuery getAllDealershipQuery)
        {
            try
            {
                return await this.mediator.Send(getAllDealershipQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveDealership")]
        public async Task<IActionResult> Save(SaveDealershipCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    var result = await this.mediator.Send(command);
                    if (result == 200)
                    {
                        return this.Result(ResponseStatus.OK, "Dealership Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Active Distributor alreay exists against territory!", null);
                    }
                    else
                    {
                        return this.Result(ResponseStatus.Error, "There is some error!", null);
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDealershipByName")]
        public async Task<ActionResult<List<GetDealership>>> GetDealershipByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetDealershipByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteDealership")]
        public async Task<ActionResult<long>> DeleteDealership(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteDealershipQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Dealership Not Exist!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Dealership!");
                }
                else if (result == (long)ResponseStatus.OK)
                {
                    return this.Result(ResponseStatus.OK, null, "Successfully Deleted!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Something went Wrong!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDealershipByTerritoryId")]
        public async Task<ActionResult<List<GetDealership>>> GetDealershipByTerritoryId(long territoryId)
        {
            try
            {
                return await this.mediator.Send(new GetDealershipByTerritoryIdQuery(territoryId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDealershipByTerritorySaleModel")]
        public async Task<ActionResult<GetDealership>> GetDealershipByTerritorySaleModel(string saleModel, long territoryId)
        {
            try
            {
                return await this.mediator.Send(new GetDealershipByTerritorySaleModelQuery(saleModel, territoryId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllDealershipList")]
        public async Task<ActionResult<Tuple<IEnumerable<GetDealership>, long>>> GetAllDealershipList(GetAllDealershipListQuery getAllDealershipQuery)
        {
            try
            {
                return await this.mediator.Send(getAllDealershipQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCustomerByName")]
        public async Task<ActionResult<List<GetDealership>>> GetCustomerByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetCustomerByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllByName")]
        public async Task<ActionResult<List<GetDealership>>> GetAllByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetAllByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
        [HttpGet]
        [Route("GetAllActiveByName")]
        public async Task<ActionResult<List<GetDealership>>> GetAllActiveByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetActiveDealershipByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
        [HttpGet]
        [Route("GetAllDistributorType")]
        public async Task<ActionResult<List<GetDistributorType>>> GetAllDistributorType()
        {
            try
            {
                //List<GetDistributorType> lobj = new List<GetDistributorType>();
                //return lobj;
                return await mediator.Send(new GetDistributorTypeQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}
