using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fullstack.LoggingandHealthchecks.Controllers
{
    [ApiController]
    [Route("health")]
    public sealed class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(
            HealthCheckService healthCheckService,
            ILogger<HealthController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }


        [HttpGet("live")]
        public IActionResult Live()
        {
            _logger.LogDebug("Liveness check requested.");

            return Ok(new
            {
                status = "Healthy",
                check = "live"
            });
        }

        [HttpGet("ready")]
        public async Task<IActionResult> Ready(
            CancellationToken cancellationToken)
        {
            var report = await _healthCheckService.CheckHealthAsync(
                registration => registration.Tags.Contains("ready"),
                cancellationToken);

            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description
                })
            };

            _logger.LogInformation(
                "Readiness check completed with status {HealthStatus}",
                report.Status);

            if (report.Status != HealthStatus.Healthy)
            {
                _logger.LogWarning(
                    "Application readiness check failed.");

                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    response);
            }

            return Ok(response);
        }
    }

}
