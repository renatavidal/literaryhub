using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

public partial class Site : MasterPage
{
    protected UserSession CurrentUser
    {
        get
        {
            return (Session != null) ? (Session["auth"] as UserSession) : null;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        var sess = (UserSession)(Session != null ? Session["auth"] : null);
        pnlAnon.Visible = (sess == null);
        pnlAuth.Visible = (sess != null);

        lnkBitacora.Visible = UsuarioActualEsAdmin();
        HyperLink3.Visible = UsuarioActualEsAdmin();


        if (sess != null)
        {
            var email = sess.Email ?? "";
            litEmail.Text = Server.HtmlEncode(email);
            litEmailSmall.Text = Server.HtmlEncode(email);
            litInitials.Text = Server.HtmlEncode(GetInitial(email));
        }
    }

    string GetInitial(string email)
    {
        if (string.IsNullOrEmpty(email)) return "?";
        var at = email.IndexOf('@');
        var user = at > 0 ? email.Substring(0, at) : email;
        return user.Length > 0 ? user.Substring(0, 1).ToUpperInvariant() : "?";
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Session.Remove("auth");
        Session.Abandon();
        System.Web.Security.FormsAuthentication.SignOut();
        Response.Redirect("/Login.aspx");
    }
    private bool UsuarioActualEsAdmin()
    {
        if( CurrentUser != null)
            return CurrentUser.IsInRole("Admin"); 
        return false;
    }


}
