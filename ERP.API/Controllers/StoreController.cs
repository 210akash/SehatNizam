using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Store.Query;
using ERP.Mediator.Mediator.Store.Command;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IMediator mediator;

        public StoreController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetStoreById")]
        public async Task<ActionResult<GetStore>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetStoreByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllStores")]
        public async Task<ActionResult<Tuple<IEnumerable<GetStore>, long>>> GetAll(GetAllStoreQuery getAllStoreQuery)
        {
            try
            {
                return await this.mediator.Send(getAllStoreQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveStore")]
        public async Task<IActionResult> Save(SaveStoreCommand command)
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
                        return this.Result(ResponseStatus.OK, "Store Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Name Already Exists!", null);
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
        [Route("GetStoreByName")]
        public async Task<ActionResult<List<GetStore>>> GetStoreByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetStoreByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteStore")]
        public async Task<ActionResult<bool>> DeleteStore(long id)
        {
            try
            {
                return await this.mediator.Send(new DeleteStoreQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetStoreByCompany")]
        public async Task<ActionResult<List<GetStore>>> GetStoreByCompany(long CompanyId, bool FixedAsset)
        {
            try
            {
                return await mediator.Send(new GetStoreByCompanyQuery(CompanyId, FixedAsset));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetStoreByLocation")]
        public async Task<ActionResult<List<GetStore>>> GetStoreByLocation(long LocationId)
        {
            try
            {
                return await mediator.Send(new GetStoreByLocationQuery(LocationId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}
