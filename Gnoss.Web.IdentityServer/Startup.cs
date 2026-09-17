using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using static OpenIddict.Server.OpenIddictServerEvents;

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
                issuerUri = Configuration["IssuerUri"];
            }

            if (string.IsNullOrEmpty(issuerUri))
            {
                throw new InvalidOperationException("La variable de configuracion 'IssuerUri' es obligatoria");
            }

            int tiempo = 0;

            if (environmentVariables.Contains("segundostoken"))
            {
                _ = int.TryParse(environmentVariables["segundostoken"] as string, out tiempo);
            }
            else
            {
                _ = int.TryParse(Configuration["segundostoken"], out tiempo);
            }

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
                scopeIdentity = Configuration["scopeIdentity"];
            }
            if (string.IsNullOrEmpty(scopeIdentity))
            {
                throw new InvalidOperationException("La variable de configuracion 'scopeIdentity' es obligatoria");
            }
            string clientIDIdentity = "";

            if (environmentVariables.Contains("clientIDIdentity"))
            {
                clientIDIdentity = environmentVariables["clientIDIdentity"] as string;
            }
            else
            {
                clientIDIdentity = Configuration["clientIDIdentity"];
            }

            if (string.IsNullOrEmpty(clientIDIdentity))
            {
                throw new InvalidOperationException("La variable de configuracion 'clientIDIdentity' es obligatoria");
            }

            string clientIDSecret = "";
            if (environmentVariables.Contains("clientSecretIdentity"))
            {
                clientIDSecret = environmentVariables["clientSecretIdentity"] as string;
            }
            else
            {
                clientIDSecret = Configuration["clientSecretIdentity"];
            }

            if (string.IsNullOrEmpty(clientIDSecret))
            {
                throw new InvalidOperationException("La variable de configuracion 'clientSecretIdentity' es obligatoria");
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

            // Base de datos en memoria
            services.AddDbContext<DbContext>(options =>
            {
                options.UseInMemoryDatabase("openiddict-db");
                options.UseOpenIddict();
            });

            // Clase que se enecarga de responder las peticiones /connect/token
            services.AddScoped<TokenRequestHandler>();

            // Configuraci�n de OpenIdDict
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                   .UseDbContext<DbContext>();
                })
                .AddServer(options =>
                {
                    // Indicamos la URI del servidor de autenticacion
                    options.SetIssuer(new Uri(issuerUri));

                    // Indicar el endpoint donde se piden los tokens
                    options.SetTokenEndpointUris("/connect/token");

                    // Se habilita el flujo de tipo client_credentials
                    options.AllowClientCredentialsFlow();

                    // Se establece el tiempo de vida del token (valor por defecto 24h)
                    options.SetAccessTokenLifetime(TimeSpan.FromSeconds(tiempo));

                    // Se genera la clave de cifrado en memoria
                    options.AddEphemeralEncryptionKey()
                           .AddEphemeralSigningKey();

                    // Deshabilita la encriptacion de los tokens
                    options.DisableAccessTokenEncryption();

                    // Monta los middlewere de ASP.Net Core para que funcione con los de OpenIddict
                    options.UseAspNetCore()
                           // Permite recibir peticiones http                        
                           .DisableTransportSecurityRequirement();

                    // Clase encargada de procesar las solicitudes al endpoint: /connect/token
                    options.AddEventHandler<HandleTokenRequestContext>(builder => builder.UseScopedHandler<TokenRequestHandler>());
                });

            // Poblar la base de datos en memoria con las configuraciones especificadas
            services.AddHostedService(sp =>
                new OpenIddictSeeder(clientIDIdentity, clientIDSecret, scopeIdentity, sp));

            services.AddControllers();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("_myAllowSpecificOrigins");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
