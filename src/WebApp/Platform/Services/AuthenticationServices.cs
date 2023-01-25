using Microsoft.Identity.Web;
using MyAppRoot.WebApp.Platform.Settings;

namespace MyAppRoot.WebApp.Platform.Services;

public static class AuthenticationServices
{
    public static void AddAuthenticationServices(
        this IServiceCollection services,
        ConfigurationManager configuration,
        bool isLocal)
    {
        var authenticationBuilder = services.AddAuthentication();

        // An Azure AD app must be registered and configured in the settings file.
        // (Currently not compatible with in-memory data.)
        if (!isLocal || ApplicationSettings.LocalDevSettings is { UseAzureAd: true, UseInMemoryData: false })
            authenticationBuilder.AddMicrosoftIdentityWebApp(configuration, cookieScheme: null);
        // Note: `cookieScheme: null` is mandatory. See https://github.com/AzureAD/microsoft-identity-web/issues/133#issuecomment-739550416
    }
}
