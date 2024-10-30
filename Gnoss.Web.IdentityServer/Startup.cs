using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Gnoss.Web.IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			ILoggerFactory loggerFactory =
			LoggerFactory.Create(builder =>
			{
				builder.AddConfiguration(Configuration.GetSection("Logging"));
				builder.AddSimpleConsole(options =>
				{
					options.IncludeScopes = true;
					options.SingleLine = true;
					options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
					options.UseUtcTimestamp = true;
				});
			});

			services.AddSingleton(loggerFactory);
			IDictionary environmentVariables = Environment.GetEnvironmentVariables();

            string issuerUri = "";
            if (environmentVariables.Contains("IssuerUri"))
            {
                issuerUri = environmentVariables["IssuerUri"] as string;
            }
            else
            {
                issuerUri = Configuration.GetConnectionString("IssuerUri");
            }

            int tiempo = 0;
            try
            {
                if (environmentVariables.Contains("segundostoken"))
                {
                    tiempo = int.Parse(environmentVariables["segundostoken"] as string);
                }
                else
                {
                    tiempo = int.Parse(Configuration.GetConnectionString("segundostoken"));
                }
            }
            catch { }
            if (tiempo == 0)
            {
                tiempo = 86400;
            }
            string scopeIdentity = "";
            if (environmentVariables.Contains("scopeIdentity"))
            {
                scopeIdentity = environmentVariables["scopeIdentity"] as string;
            }
            else
            {
                scopeIdentity = Configuration.GetConnectionString("scopeIdentity");
            }

            string clientIDIdentity = "";
            if (environmentVariables.Contains("clientIDIdentity"))
            {
                clientIDIdentity = environmentVariables["clientIDIdentity"] as string;
            }
            else
            {
                clientIDIdentity = Configuration.GetConnectionString("clientIDIdentity");
            }

            string clientIDSecret = "";
            if (environmentVariables.Contains("clientSecretIdentity"))
            {
                clientIDSecret = environmentVariables["clientSecretIdentity"] as string;
            }
            else
            {
                clientIDSecret = Configuration.GetConnectionString("clientSecretIdentity");
                
            }


            // Add Cors
            services.AddCors(options =>
            {
                options.AddPolicy(name: "_myAllowSpecificOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                });
            });
            services.AddIdentityServer(x =>
            {
                x.IssuerUri = issuerUri;
            }).AddDeveloperSigningCredential()
               .AddInMemoryPersistedGrants()
               .AddInMemoryCaching()
               .AddInMemoryClients(Clientes(tiempo, clientIDIdentity, clientIDSecret, scopeIdentity))
               .AddInMemoryApiResources(ApiResources(scopeIdentity))
               .AddInMemoryApiScopes(apiScopes(scopeIdentity));
            


            services.AddControllers();
            

        }

        private IEnumerable<ApiScope> apiScopes(string pScope)
        {
            return Config.GetApiScopes(pScope);
        }

        private IEnumerable<ApiResource> ApiResources(string pScope)
        {
            return Config.GetApiResources(pScope);
        }

        private IEnumerable<Client> Clientes(int pTiempo, string pClienteID, string pClienteSecret, string pScope)
        {
            return Config.GetClients(pTiempo, pClienteID, pClienteSecret, pScope);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
