using System;
using System.Web.UI;

public partial class AccessDenied : PublicPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.StatusCode = 403;
        Response.TrySkipIisCustomErrors = true;

        var auth = Session["auth"] as UserSession;

        if (auth != null && !string.IsNullOrEmpty(auth.Email))
            litUser.Text = " (usuario: " + Server.HtmlEncode(auth.Email) + ")";

        lnkLogin.Visible = (auth == null);
        if (lnkLogin.Visible)
        {
            var ret = Server.UrlEncode(Request.RawUrl);
            lnkLogin.NavigateUrl = "/Login.aspx?returnUrl=" + ret;
        }
    }
}
