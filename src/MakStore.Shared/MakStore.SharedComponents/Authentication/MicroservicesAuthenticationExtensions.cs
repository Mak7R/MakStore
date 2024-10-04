using MakStore.SharedComponents.Authentication.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace MakStore.SharedComponents.Authentication;

public static class MicroservicesAuthenticationExtensions
{
    public static AuthenticationBuilder AddMicroservicesAuthentication(this AuthenticationBuilder builder, Action<MicroservicesAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<MicroservicesAuthenticationOptions, MicroservicesAuthenticationHandler>(MicroservicesAuthenticationDefaults.AuthenticationScheme, configureOptions);
    }
}