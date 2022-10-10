using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using plhhoa.Services;
using Lib;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace plhhoa.HealthChecks
{
    public class CouchClientHealthCheck : IHealthCheck
    {
        private readonly UserLinkService  _userLinkService;
        public CouchClientHealthCheck(UserLinkService userLinkService)
        {
            this._userLinkService = userLinkService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _userLinkService.List("http", 1, 0);
                return HealthCheckResult.Healthy("A healthy result");
            }
            catch(Exception e){
                return HealthCheckResult.Unhealthy("An unhealthy result");
            }
        }
    }
}