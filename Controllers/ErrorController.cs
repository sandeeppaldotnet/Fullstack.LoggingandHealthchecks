using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fullstack.LoggingandHealthchecks.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public sealed class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }


        [Route("/api/error")]
        public IActionResult HandleError()
        {
            var exceptionFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var exception = exceptionFeature?.Error;

            if (exception is not null)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception for path {Path}. RequestId: {RequestId}",
                    exceptionFeature?.Path,
                    HttpContext.TraceIdentifier);
            }

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                detail: "Please try again later or contact support.",
                instance: HttpContext.Request.Path);
        }
    }

}
