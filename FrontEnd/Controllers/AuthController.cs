using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FrontEnd.Views;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class AuthController : Controller
    {

        [Route("Login")]
        public IActionResult Login(string returnURL)
        {

            var userinfo = new Login();

            try
            {
                EnsureLogOut();

                userinfo.ReturnUrl = returnURL;

                return View(userinfo);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void EnsureLogOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                Logout();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        private void Logout()
        {
            SignOut();

            //Empty the users logged in details
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // Clear the session cache to help prevent MIM attacks
            HttpContext.Session.Clear();

            RedirectToLocal();


        }

        

        private ActionResult RedirectToLocal(string returnUrl = "")
        {
            try
            {
                if(!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToPage("Index");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Route("ValidateUser")]
        public IActionResult ValidateUser(string username, string password, string redirectLocation)
        {

            //Open connection to the ms.ds active directory
            using (var pc = new PrincipalContext(ContextType.Domain, "ms.ds.uhc.com", null, ContextOptions.Negotiate))
            {
                //validate user
                if (!pc.ValidateCredentials(username, password))
                {
                    //Get user details
                    var user = UserPrincipal.FindByIdentity(pc, username);
                    //If unable to find user on the system then return forbiden
                    if (user == null)
                    {
                        return Unauthorized();
                    }
                    ///Create an identity store for the users roles
                    var id = new ClaimsIdentity();

                    id.AddClaim(new Claim(ClaimTypes.Upn, user.UserPrincipalName));
                    id.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"));
                    id.AddClaim(new Claim(ClaimTypes.Name, user.GivenName));
                    id.AddClaim(new Claim(ClaimTypes.Email, user.EmailAddress));
                    id.AddClaim(new Claim(ClaimTypes.WindowsAccountName, username));

                    var groups = user.GetGroups();

                    var roles = groups.Select(x => new Claim(ClaimTypes.Role, x.Name));
                    id.AddClaims(roles);

                    User.AddIdentity(id);

                    return RedirectToRoute(redirectLocation);
                }
                return Unauthorized();
            }
        }
    }
}