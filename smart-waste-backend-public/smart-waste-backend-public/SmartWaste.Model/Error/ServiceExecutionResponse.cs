using Microsoft.AspNetCore.Mvc;

namespace SmartWaste.Model.Error
{
    public class ServiceExecutionResponse
    {
        public bool IsError {  get; set; }
        public string? ErrorMessage { get; set; } = "";

        /// <summary>
        /// Generate response for API
        /// </summary>
        /// <returns></returns>
        public IActionResult GenerateResponse(ControllerBase controller)
        {
            if (!IsError)
                return controller.Ok();
            
            return controller.BadRequest(new HttpError(ErrorMessage ?? ""));
        }
    }
}
