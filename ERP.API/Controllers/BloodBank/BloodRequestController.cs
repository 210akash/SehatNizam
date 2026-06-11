using ERP.API.Extensions;

using ERP.BusinessModels.Enums;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.Request.Command;

using ERP.Mediator.Mediator.BloodBank.Request.Query;

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

    public class BloodRequestController : ControllerBase

    {

        private readonly IMediator mediator;



        public BloodRequestController(IMediator mediator)

        {

            this.mediator = mediator;

        }



        [HttpPost]

        [Route("GetAllBloodRequests")]

        public async Task<ActionResult<Tuple<IEnumerable<GetBloodRequest>, long>>> GetAll(GetAllBloodRequestQuery query)

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



        [HttpGet]

        [Route("GetBloodRequestById")]

        public async Task<ActionResult<GetBloodRequest>> GetById(long id)

        {

            try

            {

                return await mediator.Send(new GetBloodRequestByIdQuery(id));

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }



        [HttpGet]

        [Route("GetBloodRequestLog")]

        public async Task<ActionResult<GetBloodRequestLog>> GetLog(long id)

        {

            try

            {

                var result = await mediator.Send(new GetBloodRequestLogQuery(id));

                if (result == null)

                {

                    return this.Result(ResponseStatus.Error, null, "Blood request not found!");

                }

                return result;

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }



        [HttpPost]

        [Route("SaveBloodRequest")]

        public async Task<IActionResult> Save(SaveBloodRequestCommand command)

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

                    return this.Result(ResponseStatus.OK, "Blood Request Saved!", null);

                }

                else if (result == 400)

                {

                    return this.Result(ResponseStatus.Error, "Quantity must be at least 1!", null);

                }

                else if (result == 409)

                {

                    return this.Result(ResponseStatus.Conflict, "Only pending blood requests can be edited!", null);

                }



                return this.Result(ResponseStatus.Error, "There is some error!", null);

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }



        [HttpDelete]

        [Route("DeleteBloodRequest")]

        public async Task<ActionResult<bool>> Delete(long id)

        {

            try

            {

                return await mediator.Send(new DeleteBloodRequestQuery(id));

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }

    }

}

