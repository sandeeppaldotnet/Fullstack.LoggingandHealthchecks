using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fullstack.LoggingandHealthchecks.Health
{
    public sealed class CatalogDependencyHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CatalogDependencyHealthCheck> _logger;

        public CatalogDependencyHealthCheck(
            IConfiguration configuration,
            ILogger<CatalogDependencyHealthCheck> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var isAvailable = bool.TryParse(
                _configuration["DemoDependencies:CatalogAvailable"],
                out var available) && available;

            if (isAvailable)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy(
                        "Catalog dependency is available."));
            }

            _logger.LogWarning(
                "Catalog dependency health check failed.");

            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "Catalog dependency is unavailable."));
        }
    }

}
