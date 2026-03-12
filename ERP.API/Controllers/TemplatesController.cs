using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using ERP.BusinessModels.ParameterVM;
using ERP.Mediator.Mediator.Templates.Query;
using ERP.Mediator.Mediator.Templates.Command;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TemplatesController : BaseController
    {
        private readonly IMediator mediator;

        public TemplatesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<ActionResult<GetTemplates>> GetById(long Id)
        {
            try
            {
                return await mediator.Send(new GetTemplateByIdQuery(Id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAll")]

        public async Task<ActionResult<Tuple<IEnumerable<GetTemplates>, long>>> GetAll(GetAllTemplatesQuery getAllTemplatesQuery)
        {
            try
            {
                return await this.mediator.Send(getAllTemplatesQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save(SaveTemplatesCommand command)
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
                        return this.Result(ResponseStatus.OK, "Template Saved!", null);
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
        [Route("GetPrintTemplate")]
        public async Task<ActionResult<string>> GetPrintTemplate(long OrderId, long TemplateId, long DispatchId)
        {
            try
            {
                var result = await this.mediator.Send(new GetPrintTemplateQuery(OrderId, TemplateId, DispatchId));
                return this.Result(ResponseStatus.OK, result, null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPrintTemplateByShopId")]
        public async Task<ActionResult<string>> GetPrintTemplateByShopId(long ShopId, long TemplateId)
        {
            try
            {
                var result = await this.mediator.Send(new GetPrintTemplateByShopIdQuery(ShopId, TemplateId));
                return this.Result(ResponseStatus.OK, result, null);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
    }
}
