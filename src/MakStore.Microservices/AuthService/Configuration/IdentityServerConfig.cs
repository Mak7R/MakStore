using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;
using MakStore.SharedComponents.Constants;

namespace AuthService.Configuration;

public static class IdentityServerConfig
{
    public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email(),
        new IdentityResource
        {
            Name = "role",
            UserClaims = new List<string>
            {
                JwtClaimTypes.Role
            }
        }
    };

    public static IEnumerable<ApiScope> ApiScopes => new[]
    {
        new ApiScope(name: MicroservicesConstants.ApiDefaults.ProductsApi, displayName: "Products API", userClaims: new []{JwtClaimTypes.Email, JwtClaimTypes.Name, JwtClaimTypes.Role})
    };

    public static IEnumerable<Client> Clients => new[]
    {
        new Client
        {
            ClientId = "client",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { MicroservicesConstants.ApiDefaults.ProductsApi }
        },
        new Client
        {
            ClientId = "EmployeesWebClient",
            ClientSecrets = { new Secret("secret".Sha256()) },
            
            AllowedGrantTypes = GrantTypes.Code,
            
            RedirectUris = { "http://localhost:9002/signin-oidc", "http://employees.makstore:9002/signin-oidc", "http://employees.makstore:9000/signin-oidc" },
            PostLogoutRedirectUris = { "http://localhost:9002/signout-callback-oidc", "http://employees.makstore:9002/signout-callback-oidc", "http://employees.makstore:9000/signout-callback-oidc" },
            
            AccessTokenLifetime = 60 * 60 * 3,
            AllowOfflineAccess = true,
            
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                MicroservicesConstants.ApiDefaults.ProductsApi,
                "role"
            }
        }
    };
}