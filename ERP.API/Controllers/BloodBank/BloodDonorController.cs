using ERP.API.Extensions;

using ERP.BusinessModels.Enums;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.Donor.Command;

using ERP.Mediator.Mediator.BloodBank.Donor.Query;

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

    public class BloodDonorController : ControllerBase

    {

        private readonly IMediator mediator;



        public BloodDonorController(IMediator mediator)

        {

            this.mediator = mediator;

        }



        [HttpPost]

        [Route("GetAllBloodDonors")]

        public async Task<ActionResult<Tuple<IEnumerable<GetBloodDonor>, long>>> GetAll(GetAllBloodDonorQuery query)

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

        [Route("GetBloodDonorById")]

        public async Task<ActionResult<GetBloodDonor>> GetById(long id)

        {

            try

            {

                return await mediator.Send(new GetBloodDonorByIdQuery(id));

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }



        [HttpPost]

        [Route("SaveBloodDonor")]

        public async Task<IActionResult> Save(SaveBloodDonorCommand command)

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

                    return this.Result(ResponseStatus.OK, "Blood Donor Saved!", null);

                }

                else if (result == 409)

                {

                    return this.Result(ResponseStatus.Conflict, "CNIC Already Exists!", null);

                }



                return this.Result(ResponseStatus.Error, "There is some error!", null);

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }



        [HttpDelete]

        [Route("DeleteBloodDonor")]

        public async Task<ActionResult<bool>> Delete(long id)

        {

            try

            {

                return await mediator.Send(new DeleteBloodDonorQuery(id));

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }

    }

}

