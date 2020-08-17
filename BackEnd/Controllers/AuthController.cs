using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [Route("ValidateUser")]
        public ActionResult ValidateUser(string username, string password,string redirectLocation)
        {

            //Open connection to the ms.ds active directory
            using(var pc  = new PrincipalContext(ContextType.Domain, "ms.ds.uhc.com", null, ContextOptions.Negotiate))
            {
                //validate user
                if (!pc.ValidateCredentials(username, password))
                {
                    //Get user details
                    var user = UserPrincipal.FindByIdentity(pc, username);
                    //If unable to find user on the system then return forbiden
                    if(user == null)
                    {
                        return Unauthorized();
                    }
                    ///Create an identity store for the users roles
                    var id = new ClaimsIdentity();

                    id.AddClaim(new Claim(ClaimTypes.Upn, user.UserPrincipalName));
                    id.AddClaim(new Claim(ClaimTypes.Name, user.GivenName));
                    id.AddClaim(new Claim(ClaimTypes.Email, user.EmailAddress));
                    id.AddClaim(new Claim(ClaimTypes.WindowsAccountName, username));

                    var groups = user.GetGroups();

                    var roles = groups.Select(x => new Claim(ClaimTypes.Role, x.Name));
                    id.AddClaims(roles);

                    return RedirectToRoute(redirectLocation);
                }
                return Unauthorized();
            }
        }
    }
}