using Microsoft.AspNetCore.Builder;

namespace AcdemyDashbord.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void RefreshLogin(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await context.RefreshLoginAsync();
                await next();
            });
        }
    }
}
