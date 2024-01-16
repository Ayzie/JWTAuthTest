using Microsoft.AspNetCore.Mvc;

namespace JWTAuthTest.Utils
{
    public class ControllerExtension : Controller
    {
        public IActionResult ConsumeResult(BusinessLogicMessage result)
        {
            return StatusCode((int)result.StatusCode, result);
        }

        public string GetModelErrorMessage()
        {
            var message = "The Model is invalid with the following errors:" + Environment.NewLine;
            message += string.Join(Environment.NewLine, ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            return message;
        }
    }
}
