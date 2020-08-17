using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontEnd.Areas.AI.Controllers
{
    public class CobController : Controller
    {
        public IActionResult CobReview()
        {
            if (!User.Identity.IsAuthenticated)
            {
                Redirect("/Login");
            }
            else
            {
                if(!User.IsInRole("Cob") || !User.IsInRole("Dev"))
                {
                    Redirect("/AccessDenied");
                }
            }

            return RedirectToPage("CobReview");
            
        }
    }
}