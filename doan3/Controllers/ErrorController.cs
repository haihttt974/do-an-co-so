using Microsoft.AspNetCore.Mvc;

namespace doan3.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View("NotFound");
        }

        [Route("Error/403")]
        public IActionResult Error403()
        {
            return View("AccessDenied");
        }

        [Route("Error/500")]
        public IActionResult Error500()
        {
            return View("ServerError");
        }

        [Route("Error/{statusCode}")]
        public IActionResult GenericError(int statusCode)
        {
            switch (statusCode)
            {
                case 404: return RedirectToAction("Error404");
                case 403: return RedirectToAction("Error403");
                case 500: return RedirectToAction("Error500");
                default:
                    ViewBag.ErrorMessage = $"Lỗi {statusCode} không xác định.";
                    return View("GenericError");
            }
        }
    }
}
