using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontEnd.Areas.Cob.Pages
{
    public class CobReviewModel : PageModel
    {
        public void OnGet()
        {
            if(!User.IsInRole("Cob") || !User.IsInRole("Dev"))
            {
                RedirectToPage("/AccessDenied");
            }
        }
    }
}
