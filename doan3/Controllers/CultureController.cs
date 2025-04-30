using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace doan3.Controllers
{
    public class CultureController : Controller
    {
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            // Lưu ngôn ngữ vào cookie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) }
            );

            // Chuyển về trang trước đó hoặc trang mặc định
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
