using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using ERP.Mediator.Mediator.SalesTarget.Query;
using ERP.Mediator.Mediator.SalesTarget.Command;
using static ERP.Mediator.Mediator.SalesTarget.Handler.GetSalesTargetByZoneIdHandler;
using static ERP.Mediator.Mediator.SalesTarget.Handler.GetDSFTargetsByTerritoryIdHandler;
using static ERP.Mediator.Mediator.SalesTarget.Handler.GetTargetsByTerritoryIdHandler;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesTargetController : ControllerBase
    {
        private readonly IMediator mediator;

        public SalesTargetController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetSalesTargetByZoneId")]
        public async Task<ActionResult<List<GroupedSalesTarget>>> GetSalesTargetByZoneId(long zoneId, DateTime targetMonth)
        {
            try
            {
                return await this.mediator.Send(new GetSalesTargetByZoneIdQuery(zoneId, targetMonth));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllSalesTarget")]
        public async Task<ActionResult<Tuple<IEnumerable<GetSalesTarget>, long>>> GetAll(GetAllSalesTargetQuery getAllSalesTargetQuery)
        {
            try
            {
                return await this.mediator.Send(getAllSalesTargetQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSalesTarget")]
        public async Task<IActionResult> SaveSalesTarget(SaveSalesTargetCommand command)
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
                        return this.Result(ResponseStatus.OK, "Sales Target Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Target Already Exists!", null);
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
        [Route("DeleteSalesTarget")]
        public async Task<ActionResult<long>> DeleteSalesTarget(long zoneId, DateTime targetMonth)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteSalesTargetQuery(zoneId, targetMonth));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Sales Target Not Exist!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting SalesTarget!");
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
        [Route("IsZoneTargetExist")]
        public async Task<ActionResult<bool>> IsZoneTargetExist(long zoneId)
        {
            try
            {
                var result = await this.mediator.Send(new IsZoneTargetExistQuery(zoneId));
                return result;
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveTerritoryTarget")]
        public async Task<IActionResult> SaveTerritoryTarget(SaveTerritoryTargetCommand command)
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
                        return this.Result(ResponseStatus.OK, "Territory Target Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Target Already Exists!", null);
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
        [Route("GetTerritoryTargetsByZoneId")]
        public async Task<ActionResult<List<GetSalesTarget>>> GetTerritoryTargetsByZoneId(long ZoneId, DateTime TargetMonth)
        {
            try
            {
                return await this.mediator.Send(new GetTerritoryTargetsByZoneIdQuery(ZoneId, TargetMonth));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveDSFTarget")]
        public async Task<IActionResult> SaveDSFTarget(SaveDSFTargetCommand command)
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
                        return this.Result(ResponseStatus.OK, "DSF Target Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "DSF Already Exists!", null);
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
        [Route("GetDSFTargetsByTerritoryId")]
        public async Task<ActionResult<List<UserTerritoryTargetDto>>> GetDSFTargetsByTerritoryId(long TerritoryId, DateTime TargetMonth)
        {
            try
            {
                return await this.mediator.Send(new GetDSFTargetsByTerritoryIdQuery(TerritoryId, TargetMonth));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTargetsByTerritoryId")]
        public async Task<ActionResult<GetTerritoryTarget>> GetTargetsByTerritoryId(long TerritoryId, DateTime TargetMonth)
        {
            try
            {
                return await this.mediator.Send(new GetTargetsByTerritoryIdQuery(TerritoryId, TargetMonth));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}
