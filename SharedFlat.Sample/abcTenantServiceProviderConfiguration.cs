﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SharedFlat.Sample
{
    public sealed class abcTenantServiceProviderConfiguration : ITenantConfiguration
    {
        public void Configure(IConfiguration configuration)
        {
            configuration["Tenant"] = this.Tenant;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<Xpto>(sp => new Xpto { Name = this.Tenant });
        }

        public string Tenant { get; } = "abc";
    }
}
