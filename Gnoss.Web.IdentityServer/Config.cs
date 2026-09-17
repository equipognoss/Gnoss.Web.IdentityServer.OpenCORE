using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Gnoss.Web.IdentityServer;

public static class Config
{
    public static OpenIddictApplicationDescriptor GetClient(
        string clientId, string clientSecret, string scope)
    {
        return new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            ClientType = ClientTypes.Confidential,
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.ClientCredentials,
                Permissions.Prefixes.Scope + scope,
            }
        };
    }

    public static OpenIddictScopeDescriptor GetScope(string scope)
    {
        return new OpenIddictScopeDescriptor
        {
            Name = scope,
            DisplayName = "Identity Server API",
            Resources = { scope }
        };
    }
}