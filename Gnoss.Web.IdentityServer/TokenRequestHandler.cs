using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace Gnoss.Web.IdentityServer;

public class TokenRequestHandler : IOpenIddictServerHandler<HandleTokenRequestContext>
{
    private readonly IOpenIddictScopeManager _scopeManager;

    public TokenRequestHandler(IOpenIddictScopeManager scopeManager)
    {
        _scopeManager = scopeManager;
    }

    public async ValueTask HandleAsync(HandleTokenRequestContext context)
    {
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaim(new Claim(Claims.Subject, context.ClientId!));

        var principal = new ClaimsPrincipal(identity);

        principal.SetScopes(context.Request.GetScopes());

        var resources = await _scopeManager
            .ListResourcesAsync(principal.GetScopes())
            .ToListAsync();

        principal.SetResources(resources);

        context.SignIn(principal);
    }
}