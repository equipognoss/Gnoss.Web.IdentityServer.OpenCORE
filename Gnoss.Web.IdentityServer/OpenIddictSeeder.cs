using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gnoss.Web.IdentityServer;

/// <summary>
/// Registra los clientes y scopes en el store in-memory al arrancar la app.
/// </summary>
public class OpenIddictSeeder : IHostedService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _scope;
    private readonly IServiceProvider _services;

    public OpenIddictSeeder(
        string clientId, string clientSecret,
        string scope, IServiceProvider services)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _scope = scope;
        _services = services;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _services.CreateAsyncScope();

        var scopeManager = scope.ServiceProvider
            .GetRequiredService<IOpenIddictScopeManager>();

        if (await scopeManager.FindByNameAsync(_scope, cancellationToken) is null)
        {
            await scopeManager.CreateAsync(
                Config.GetScope(_scope), cancellationToken);
        }

        var appManager = scope.ServiceProvider
            .GetRequiredService<IOpenIddictApplicationManager>();

        if (await appManager.FindByClientIdAsync(_clientId, cancellationToken) is null)
        {
            await appManager.CreateAsync(
                Config.GetClient(_clientId, _clientSecret, _scope),
                cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}