using ERP.API.Extensions;

using ERP.BusinessModels.Enums;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.Fridge.Command;

using ERP.Mediator.Mediator.BloodBank.Fridge.Query;

using MediatR;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using System;

using System.Collections.Generic;

using System.Threading.Tasks;



namespace ERP.API.Controllers.BloodBank

{

    [Route("api/[controller]")]

    [ApiController]

    [Authorize]

    public class BloodFridgeController : ControllerBase

    {

        private readonly IMediator mediator;



        public BloodFridgeController(IMediator mediator)

        {

            this.mediator = mediator;

        }



        [HttpPost]

        [Route("GetAllFridges")]

        public async Task<ActionResult<Tuple<IEnumerable<GetBloodFridge>, long>>> GetAll(GetAllFridgeQuery query)

        {

            try

            {

                return await mediator.Send(query);

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }



        [HttpPost]

        [Route("SaveFridge")]

        public async Task<IActionResult> Save(SaveFridgeCommand command)

        {

            try

            {

                if (!ModelState.IsValid)

                {

                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));

                }



                var result = await mediator.Send(command);

                if (result == 200)

                {

                    return this.Result(ResponseStatus.OK, "Fridge Saved!", null);

                }

                else if (result == 409)

                {

                    return this.Result(ResponseStatus.Conflict, "Name Already Exists!", null);

                }



                return this.Result(ResponseStatus.Error, "There is some error!", null);

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }



        [HttpDelete]

        [Route("DeleteFridge")]

        public async Task<ActionResult<bool>> Delete(long id)

        {

            try

            {

                return await mediator.Send(new DeleteFridgeQuery(id));

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }

    }

}

