using ERP.API.Extensions;

using ERP.BusinessModels.Enums;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.Donation.Command;

using ERP.Mediator.Mediator.BloodBank.Donation.Query;

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

    public class BloodDonationController : ControllerBase

    {

        private readonly IMediator mediator;



        public BloodDonationController(IMediator mediator)

        {

            this.mediator = mediator;

        }



        [HttpPost]

        [Route("GetAllBloodDonations")]

        public async Task<ActionResult<Tuple<IEnumerable<GetBloodDonation>, long>>> GetAll(GetAllBloodDonationQuery query)

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

        [Route("GetBloodDonationById")]

        public async Task<ActionResult<GetBloodDonation>> GetById(long id)

        {

            try

            {

                return await mediator.Send(new GetBloodDonationByIdQuery(id));

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }



        [HttpPost]

        [Route("SaveBloodDonation")]

        public async Task<IActionResult> Save(SaveBloodDonationCommand command)

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

                    return this.Result(ResponseStatus.OK, "Blood Donation Saved!", null);

                }

                else if (result == 409)

                {

                    return this.Result(ResponseStatus.Conflict, "Screening status cannot be changed after blood storage is assigned!", null);

                }



                return this.Result(ResponseStatus.Error, "There is some error!", null);

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }



        [HttpDelete]

        [Route("DeleteBloodDonation")]

        public async Task<ActionResult<bool>> Delete(long id)

        {

            try

            {

                return await mediator.Send(new DeleteBloodDonationQuery(id));

            }

            catch (Exception ex)

            {

                return this.Result(ResponseStatus.Error, null, ex.Message);

            }

        }

    }

}

