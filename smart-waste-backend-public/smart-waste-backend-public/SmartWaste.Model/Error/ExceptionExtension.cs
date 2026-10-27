using SmartWaste.Model.Error;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SmartWaste.Model.Error
{
    public static class ExceptionExtension
    {
        public static IActionResult GenerateBadRequest(this Exception ex, ILogger logger, ControllerBase controller, string loggerMessage, string exceptionMessage)
        {
            using (logger.BeginScope(("full_message", JsonConvert.SerializeObject(ex.ToString()))))
            {
                logger.LogError(loggerMessage);
            }
            return controller.BadRequest(new HttpError(exceptionMessage));
        }
    }
}
