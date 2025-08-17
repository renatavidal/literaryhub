using System;
using System.Web;
using System.Web.Security;

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        Session.Clear();
        Session.Abandon();


        FormsAuthentication.SignOut();
        var authCookie = FormsAuthentication.FormsCookieName;
        if (Request.Cookies[authCookie] != null)
        {
            var c = new HttpCookie(authCookie, "") { Expires = DateTime.UtcNow.AddDays(-1) };
            Response.Cookies.Add(c);
        }

      
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
    }
}
