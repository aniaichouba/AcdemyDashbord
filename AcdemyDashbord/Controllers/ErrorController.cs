using AcdemyDashbord.Models.Types;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace AcdemyDashbord.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("/Error/{StatusCode}")]
        public IActionResult Index(int StatusCode)
        {
            StatusResult model = new StatusResult();
            var statusResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (StatusCode)
            {
                case 404:
                    {
                        model.Message = "Sorry , The resource you requested could not be found.";
                        this.logger.LogWarning($"404 error accured. Path = {statusResult.OriginalPath} and QueryString = {statusResult.OriginalQueryString}");
                    }
                    break;
                default:
                    {
                        model.Message = "This page cannot be found";
                        this.logger.LogWarning($"{StatusCode} error accured. Path = {statusResult.OriginalPath} and QueryString = {statusResult.OriginalQueryString}");
                    }
                    break;
            }
            ViewBag.StatusCode = StatusCode.ToString();
            return View("NotFound", model);
        }

        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionStatus = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            this.logger.LogError($"The Path : {exceptionStatus.Path} throw an exception {exceptionStatus.Error}");
            return View();
        }
    }
}
