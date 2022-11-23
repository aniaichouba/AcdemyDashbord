using AcdemyDashbord.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace AcdemyDashbord.Tools
{
    public class RefreshLoginFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context,
                                                 ActionExecutionDelegate next)
        {
            await context.HttpContext.RefreshLoginAsync();
            await next();
        }
    }
}
