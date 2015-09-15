using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Useful_Classes
{
    public static class FormsAuth
    {
        public static void AuthenticateUser(string Username, bool IsPersistent, string commaSeperatedRoles, int Timeout, HttpContextBase me)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                                                        Username,
                                                        DateTime.Now,
                                                        DateTime.Now.AddMinutes(Timeout),
                                                        IsPersistent,
                                                        commaSeperatedRoles,
                                                        FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            me.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            // Redirect back to original URL.
            me.Response.Redirect(FormsAuthentication.GetRedirectUrl(Username, IsPersistent));
        }
    }
}
