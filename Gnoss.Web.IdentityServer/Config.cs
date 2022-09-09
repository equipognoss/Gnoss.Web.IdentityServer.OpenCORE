using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gnoss.Web.IdentityServer
{
    public class Config
    {
        /// <summary>
        /// Obtiene los clientes configurados
        /// </summary>
        public static IEnumerable<Client> GetClients(int pTiempo, string pClienteID, string pClienteSecret, string pScope)
        {
            return new List<Client>
            {
                // ClientId es el parámetro del servicio que solicita acceso a través de 'client_id'
                // ClientSecrets es el parámetro del servicio que solicita acceso a través de 'client_secret'
                // AllowedScopes son los scopes a los que se le da acceso al servicio
                
                // Cliente para el servicio externo (en desuso)
                new Client
                {
                    ClientId = pClienteID,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret(pClienteSecret.Sha256())
                    },
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        pScope
                        
                    },
                    AccessTokenLifetime = pTiempo,
                    AccessTokenType = AccessTokenType.Jwt
                },
            };
        }

        // APIs allowed to access the Auth server
        public static IEnumerable<ApiResource> GetApiResources(string pScope)
        {
            return new List<ApiResource>
            {
                new ApiResource()
                {
                    Name = pScope,   //This is the name of the API
                    Description = "This is the invoice Api-resource description",
                    Enabled = true,
                    DisplayName = "Identity Server API",
                    Scopes = new List<string> { pScope }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes(string pScope)
        {
            return new[]
            {
                new ApiScope(pScope, "Identity Server API"),
            };
        }

    }
}
