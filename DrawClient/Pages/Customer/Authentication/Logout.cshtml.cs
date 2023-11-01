using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayPal.Api;

namespace DrawClient.Pages.Customer.Authentication
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            string? learnerLogged = HttpContext.Session.GetString("learnerLogged");
            if (learnerLogged == "logged")
            {
                HttpContext.Session.SetString("learnerToken", "");
                HttpContext.Session.SetString("learnerLogged", "");
                HttpContext.Session.SetInt32("learnerId", 0);
                HttpContext.Session.SetInt32("cartQty", 0);
                return RedirectToPage("/Index");
            }
            else
            {
                return RedirectToPage("/Customer/Authentication/Login");
            }
        }
    }
}
