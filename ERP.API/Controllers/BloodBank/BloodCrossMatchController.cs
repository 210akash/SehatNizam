using ERP.API.Extensions;

using ERP.BusinessModels.Enums;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.CrossMatch.Command;

using ERP.Mediator.Mediator.BloodBank.CrossMatch.Query;

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

    public class BloodCrossMatchController : ControllerBase

    {

        private readonly IMediator mediator;



        public BloodCrossMatchController(IMediator mediator)

        {

            this.mediator = mediator;

        }



        [HttpPost]

        [Route("GetAllBloodCrossMatches")]

        public async Task<ActionResult<Tuple<IEnumerable<GetBloodCrossMatch>, long>>> GetAll(GetAllBloodCrossMatchQuery query)

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

        [Route("GetBloodCrossMatchWorklist")]

        public async Task<ActionResult<Tuple<IEnumerable<GetBloodCrossMatchWorklist>, long>>> GetWorklist(GetBloodCrossMatchWorklistQuery query)

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

        [Route("SaveBloodCrossMatch")]

        public async Task<IActionResult> Save(SaveBloodCrossMatchCommand command)

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

                    return this.Result(ResponseStatus.OK, "Blood Cross Match Saved!", null);

                }

                else if (result == 400)

                {

                    return this.Result(ResponseStatus.Error, "Invalid cross match data!", null);

                }

                else if (result == 409)

                {

                    return this.Result(ResponseStatus.Conflict, "Request or unit is not available for this action!", null);

                }



                return this.Result(ResponseStatus.Error, "There is some error!", null);

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }



        [HttpDelete]

        [Route("DeleteBloodCrossMatch")]

        public async Task<IActionResult> Delete(long id)

        {

            try

            {

                var result = await mediator.Send(new DeleteBloodCrossMatchQuery(id));

                if (result == 200)

                {

                    return this.Result(ResponseStatus.OK, true, "Successfully Deleted!");

                }

                else if (result == 404)

                {

                    return this.Result(ResponseStatus.Error, null, "Cross match not found!");

                }

                else if (result == 409)

                {

                    return this.Result(ResponseStatus.Conflict, null, "Cannot delete cross match linked to a blood issue!");

                }



                return this.Result(ResponseStatus.Error, null, "There is some error!");

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }

    }

}


