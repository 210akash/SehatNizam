//-----------------------------------------------------------------------
// <copyright file="ErrorHandlingMiddleware.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.API.Middleware
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using SendGrid.Helpers.Errors.Model;
    using ERP.Mediator.Mediator.Error.Command;
    using ERP.Services.Exceptions;

    /// <summary>
    /// Error Handling middle ware
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        /// <summary>
        /// The next
        /// </summary>
        private readonly RequestDelegate next;
        private IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <returns>Nothing return</returns>
        public ErrorHandlingMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            this.next = next;
        }

        /// <summary>
        /// Invokes the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>Nothing return</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await this.next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var user = httpContext.User.Claims.FirstOrDefault(o => o.Type == "UserId");
                    var model = new AddErrorCommand()
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace,
                        UserId = user != null ? new Guid(user.Value) : (Guid?)null
                    };
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var sensorHistory = await mediator.Send(model);
                }
            }
        }

        /// <summary>
        /// Handles the exception asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ex">The EX.</param>
        /// <returns>Nothing return</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            object result = null;
            switch (ex)
            {
                case UnauthorizedException _:
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    result = new
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        Message = "You are not allowed to execute this operation.",
                        Data = (string)null
                    };
                    break;
                case GeneralException _:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    result = new
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = ex.Message,
                        Data = (string)null
                    };
                    break;
                case DuplicateFoundException _:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    result = new
                    {
                        StatusCode = StatusCodes.Status409Conflict,
                        Message = "You are trying to make request that can cause duplication",
                        Data = (string)null
                    };
                    break;
                case EntityNotFoundException _:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    result = new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = string.IsNullOrEmpty(ex.Message) ? "You are trying to make request that does not exist" : ex.Message,
                        Data = (string)null
                    };
                    break;
                case ValidationException validationException:
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    result = new
                    {
                        Message = "Failed due to errors.",
                        errors = validationException.Errors.Select(x => new
                        {
                            x.PropertyName,
                            x.ErrorMessage
                        })
                    };
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    result = new
                    {
                        Type = "General Exception",
                        Exception = new
                        {
                            ex.Message,
                            Inner = ex.InnerException
                        }
                    };
                    break;
            }

            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
