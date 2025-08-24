using System;
using System.Web.Security;
using System.Web.UI;

public partial class Site : MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        var sess = (UserSession)(Session != null ? Session["auth"] : null);

        if (sess == null)
        {
            // No logueado
            pnlAnon.Visible = true;
            pnlAuth.Visible = false;
            return;
        }

        // Logueado
        pnlAnon.Visible = false;
        pnlAuth.Visible = true;

        var email = sess.Email ?? "";
        litEmail.Text = Server.HtmlEncode(email);
        litEmailSmall.Text = Server.HtmlEncode(email);
        litInitials.Text = Server.HtmlEncode(GetInitial(email));

        // Si no está verificado, mostrás el atajo (opcional)
        pnlVerifyShortcut.Visible = !sess.EmailVerified;
    }
    private string GetInitial(string email)
    {
        if (string.IsNullOrEmpty(email)) return "?";
        int at = email.IndexOf('@');
        string user = at > 0 ? email.Substring(0, at) : email;
        return user.Length > 0 ? user.Substring(0, 1).ToUpperInvariant() : "?";
    }
    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Session.Remove("auth");
        Session.Abandon();
        FormsAuthentication.SignOut();

        Response.Redirect("/Login.aspx");
    }
    protected void ddlLangTop_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Si manejás i18n, guardá la preferencia en sesión/cookie
        // Ejemplo:
        // Session["lang"] = ddlLangTop.SelectedValue;
        // Response.Redirect(Request.RawUrl); // opcional, recargar para aplicar
    }

}
