using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Command;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Mediator.Mediator.PrimaryOrder.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IMediator mediator;

        public AppointmentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllAppointments")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAppointment>, long>>> GetAllAppointments(GetAllAppointmentQuery command)
        {
            try
            {
                return await this.mediator.Send(command);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllAppointmentByDoctor")]
        public async Task<ActionResult<Tuple<IEnumerable<GetAppointment>, long>>> GetAllAppointmentByDoctor(GetAllAppointmentByDoctorQuery command)
        {
            try
            {
                return await this.mediator.Send(command);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllAppointmentStatus")]
        public async Task<ActionResult<List<GetAppointmentStatus>>> GetAllAppointmentStatus()
        {
            try
            {
                return await this.mediator.Send(new GetAllAppointmentStatusQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAppointmentByToken")]
        public async Task<ActionResult<List<GetAppointment>>> GetAppointmentByToken(string Token, long StatusId)
        {
            try
            {
                return await this.mediator.Send(new GetAppointmentByTokenQuery(Token, StatusId));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAppointment")]
        public async Task<IActionResult> Save(SaveAppointmentCommand command)
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
                    if (result.Item1 == 200)
                    {
                        var appoinment = await this.mediator.Send(new GetAppoinmentByIdQuery(result.Item2.Value));
                        return this.Result(ResponseStatus.OK, appoinment, "Appointment Saved!");
                    }
                    else if (result.Item1 == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, null, "Name Already Exists!");
                    }
                    else if (result.Item1 == 404)
                    {
                        return this.Result(ResponseStatus.RecordNotFound, null, "Record Not Found!");
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
        [Route("SaveConsultation")]
        public async Task<IActionResult> SaveConsultation(SaveConsultationCommand command)
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
                        return this.Result(ResponseStatus.OK, "Consultation Saved!", null);
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

        [HttpPost]
        [Route("ConfirmAppointment")]
        public async Task<ActionResult<Tuple<long, string>>> ConfirmAppointment(ConfirmAppoinmentQuery confirmAppoinmentQuery)
        {
            try
            {
                var result = await this.mediator.Send(confirmAppoinmentQuery);
                if (result.Item1 == 200)
                {
                    var appoinment =  await this.mediator.Send(new GetAppoinmentByIdQuery(confirmAppoinmentQuery.Id));
                    return this.Result(ResponseStatus.OK, appoinment, "Consultation Saved!" );
                }
                else if (result.Item1 == 409)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Name Already Exists!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "\"There is some error!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("CancelAppoinment")]
        public async Task<ActionResult<Tuple<long, string>>> CancelAppoinment(long id)
        {
            try
            {
                return await this.mediator.Send(new CancelAppoinmentQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAppoinmentById")]
        public async Task<ActionResult<GetAppointment>> GetAppoinmentById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetAppoinmentByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAppointmentsByBookingNo")]
        public async Task<ActionResult<List<GetAppointment>>> GetAppointmentsByBookingNo(string BookingNo)
        {
            try
            {
                return await this.mediator.Send(new GetAppointmentsByBookingNoQuery(BookingNo));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}
