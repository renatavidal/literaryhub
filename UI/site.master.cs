using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Site : MasterPage
{
    protected UserSession CurrentUser
    {
        get
        {
            return (Session != null) ? (Session["auth"] as UserSession) : null;
        }
    }
    private TranslationService _tr;
    protected void Page_Init(object sender, EventArgs e)
    {
        var apiKey = System.Configuration.ConfigurationManager.AppSettings["GoogleTranslateApiKey"];
        _tr = new TranslationService(apiKey);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;


        var sess = (UserSession)(Session != null ? Session["auth"] : null);
        pnlAnon.Visible = (sess == null);
        pnlAuth.Visible = (sess != null);

        lnkBitacora.Visible = UsuarioActualEsAdmin();
        HyperLink.Visible = (sess != null);
        HyperLink3.Visible = UsuarioActualEsAdmin();
        HyperLink5.Visible = (sess != null);
        HyperLink8.Visible = UsuarioActualEsAdmin();
        HyperLink1.Visible = (sess != null);
        HyperLink2.Visible = UsuarioActualEsAdmin();
        HyperLink4.Visible = UsuarioActualEsAdmin();
        HyperLink7.Visible = UsuarioActualEsAdmin();
        HyperLink9.Visible = UsuarioActualEsAdmin();
        HyperLink10.Visible = UsuarioActualEsAdmin();
        HyperLink12.Visible = UsuarioActualEsAdmin();
        HyperLink11.Visible = sess != null;

        string lang = "es";
        HttpCookie c = (Request != null) ? Request.Cookies["lh-lang"] : null;
        if (c != null && !string.IsNullOrEmpty(c.Value))
        {
            lang = c.Value;
        }
        var it = ddlLangTop.Items.FindByValue(lang);
        if (it != null) ddlLangTop.ClearSelection();
        if (it != null) it.Selected = true;

        if (sess != null)
        {
            var email = sess.Email ?? "";
            litEmail.Text = Server.HtmlEncode(email);
            litEmailSmall.Text = Server.HtmlEncode(email);
            litInitials.Text = Server.HtmlEncode(GetInitial(email));
        }
    }

    // Recargar misma URL para que ApplyCulture corra con el nuevo idioma
    protected void ddlLangTop_SelectedIndexChanged(object sender, EventArgs e)
    {
        var lang = ddlLangTop.SelectedValue;     

        var cookie = new HttpCookie("lh-lang", lang);
        cookie.Path = "/";
        cookie.Expires = DateTime.UtcNow.AddYears(1);
        Response.Cookies.Add(cookie);

        Response.Redirect(Request.RawUrl, endResponse: false);
        Context.ApplicationInstance.CompleteRequest();
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
    private bool UsuarioActualEsCliente()
    {
        if (CurrentUser != null)
            return CurrentUser.IsInRole("Client");
        return false;
    }


}
