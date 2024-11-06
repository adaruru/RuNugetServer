using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace StaticEmbeddedCore;
public static class DependencyInjection
{
    public static IApplicationBuilder UseStaticEmbedded(this IApplicationBuilder app)
    {
        app.UseFileServer(new FileServerOptions
        {
            RequestPath = "",
            FileProvider = new ManifestEmbeddedFileProvider(typeof(DependencyInjection).Assembly, "root")
        });
        return app;
    }
}