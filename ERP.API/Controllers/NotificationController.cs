using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ERP.BusinessModels.ResponseVM;
using MediatR;
using ERP.Mediator.Mediator.Notification.Query;
using ERP.Mediator.Mediator.Notification.Command;
using GetEmployeeNotificationsQuery = ERP.Mediator.Mediator.Notification.Query.GetEmployeeNotificationsQuery;
using Microsoft.AspNetCore.Authorization;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator mediator;

        public NotificationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllNotifications")]
        public async Task<ActionResult<Tuple<IEnumerable<GetNotification>, long>>> GetAll(GetAllNotificationsQuery getAllNotificationsQuery)
        {
            try
            {
                return await this.mediator.Send(getAllNotificationsQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveNotification")]
        public async Task<IActionResult> Save(SaveNotificationCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                var result = await this.mediator.Send(command);
                
                if (result == 200)
                {
                    return this.Result(ResponseStatus.OK, "Notification Saved!", null);
                }
                else if (result == 400)
                {
                    return this.Result(ResponseStatus.Error, null, "Invalid data. Please check required fields and ensure Expire Date is not in the past.");
                }
                else if (result == 409)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Notification with this title already exists for the selected department!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "There was an error saving the notification.");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteNotification")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteNotificationCommand(id));
                if (result)
                {
                    return this.Result(ResponseStatus.OK, "Notification Deleted!", null);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Notification not found!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetEmployeeNotifications")]
        public async Task<ActionResult<IEnumerable<GetNotification>>> GetEmployeeNotifications()
        {
            try
            {
                var result = await this.mediator.Send(new GetEmployeeNotificationsQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}
