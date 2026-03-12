using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Region.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.Mediator.Mediator.PriceGroup.Command;
using ERP.Mediator.Mediator.PriceGroup.Query;
using ERP.Mediator.Mediator.PrimaryOrder.Command;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PricingGroupController : ControllerBase
    {
        private readonly IMediator mediator;

        public PricingGroupController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetRegionById")]
        public async Task<ActionResult<GetRegion>> GetById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetRegionByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllPricingGroup")]
        public async Task<ActionResult<Tuple<IEnumerable<GetPriceGroup>, long>>> GetAll(GetAllPriceGroupQuery getAllPriceGroupQuery)
        {
            try
            {
                return await this.mediator.Send(getAllPriceGroupQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SavePricingGroup")]
        public async Task<IActionResult> Save(SavePriceGroupCommand command)
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
                        return this.Result(ResponseStatus.OK, "Region Saved!", null);
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
        [Route("GetRegionByName")]
        public async Task<ActionResult<List<GetRegion>>> GetRegionByName(string Name)
        {
            try
            {
                return await mediator.Send(new GetRegionByNameQuery(Name));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("DeletePricingGroup")]
        public async Task<ActionResult<long>> DeletePricingGroup(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeletePriceGroupQuery(id));
                if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Pricing Group!");
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
        [Route("GetProductGroupDetailsByGroupId")]
        public async Task<ActionResult<GetItemGroupDetails>> GetProductGroupDetailsByGroupId(long GroupId)
        {
            try
            {
                var result = await this.mediator.Send(new GetItemGroupDetailsByGroupIdQuery(GroupId));
                if (result != null && result.Count > 0)
                {
                    return this.Result(ResponseStatus.OK, result, result.Count.ToString());
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
        [HttpPost]
        [Route("SaveProductPricingDetails")]
        public async Task<IActionResult> SaveProductPricingDetails([FromBody] SavePriceGroupDetailsCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                var result = await this.mediator.Send(command);
                if (result == (int)ResponseStatus.OK)
                {
                    return this.Result(ResponseStatus.OK, "Product Pricing Saved Successfully!", null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, "Error saving product pricing!", null);
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
        [HttpGet]
        [Route("GetAllDistributorByGroupId")]
        public async Task<ActionResult<List<GetAllDistributorByGroupId>>> GetAllDistributorByGroupId(long GroupId)
        {
            try
            {
                var result = await this.mediator.Send(new GetAllDistributorByGroupIdQuery(GroupId));
                if (result != null)
                {
                    return this.Result(ResponseStatus.OK, result, result.Count.ToString()); ;
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
        [HttpPost]
        [Route("SaveDistributorPricingGroup")]
        public async Task<IActionResult> SaveDistributorPricingGroup(SaveDistributorPricingGroupCommand command)
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
                        return this.Result(ResponseStatus.OK, "Record Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Some exception has been Occure!", null);
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

        [HttpPost]
        [Route("CopyPriceGroup")]
        public async Task<IActionResult> CopyPriceGroup(CopyPriceGroupCommand command)
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
                        return this.Result(ResponseStatus.OK, "Price Group Copied!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Title Already Exists!", null);
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

    }
}
