using MyApp.WebApp.Platform.Settings;

namespace MyApp.WebApp.Platform.AppConfiguration;

internal static class SecurityHeaders
{
    private static readonly string ReportUri =
        $"https://report-to-api.raygun.com/reports?apikey={AppSettings.RaygunSettings.ApiKey}";

    internal static void AddSecurityHeaderPolicies(this HeaderPolicyCollection policies)
    {
        policies.AddFrameOptionsDeny();
        policies.AddContentTypeOptionsNoSniff();
        policies.AddReferrerPolicyStrictOriginWhenCrossOrigin();
        policies.RemoveServerHeader();
        policies.AddContentSecurityPolicyReportOnly(builder => builder.CspBuilder());
        policies.AddCrossOriginOpenerPolicy(builder => builder.SameOrigin());
        policies.AddCrossOriginEmbedderPolicy(builder => builder.Credentialless());
        policies.AddCrossOriginResourcePolicy(builder => builder.SameSite());

        if (string.IsNullOrEmpty(AppSettings.RaygunSettings.ApiKey)) return;
        policies.AddReportingEndpoints(builder => builder.AddEndpoint("csp-endpoint", ReportUri));
    }

    private static void CspBuilder(this CspBuilder builder)
    {
        builder.AddObjectSrc().None();
        builder.AddFormAction().Self();
        builder.AddFrameAncestors().None();

        if (string.IsNullOrEmpty(AppSettings.RaygunSettings.ApiKey)) return;
        builder.AddReportUri().To(ReportUri);
        builder.AddReportTo("csp-endpoint");
    }
}
