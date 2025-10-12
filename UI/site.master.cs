﻿using System;
using System.Linq;
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
        if (!IsPostBack)
        {
            var lang = GetLangFromCookie();
            if (ddlLangTop.Items.FindByValue(lang) != null)
                ddlLangTop.SelectedValue = lang;

            RegisterApplyLanguageScript(lang);
        }

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
        lnkBackups.Visible = UsuarioActualEsAdmin();
        HyperLink15.Visible = UsuarioActualEsAdmin();
        HyperLink11.Visible = sess != null && !UsuarioActualEsAdmin();
        upSearch.Visible = sess != null;

        if (sess != null)
        {
            var email = sess.Email ?? "";
            litEmail.Text = Server.HtmlEncode(email);
            litEmailSmall.Text = Server.HtmlEncode(email);
            litInitials.Text = Server.HtmlEncode(GetInitial(email));
        }
    }



    private string GetLangFromCookie()
    {
        var c = Request.Cookies["lh-lang"];
        var v = (c != null && !string.IsNullOrEmpty(c.Value)) ? c.Value : "es";
        return v;
    }

    private void SaveLangCookie(string lang)
    {
        var cookie = new HttpCookie("lh-lang", lang) { Path = "/", Expires = DateTime.UtcNow.AddYears(1) };
        Response.Cookies.Add(cookie);
        Session["lh-lang"] = lang;
    }

    private void RegisterApplyLanguageScript(string lang)
    {
        var js = "window.lhApplyGoogleLang && window.lhApplyGoogleLang('"+lang+"');";
        if (ScriptManager.GetCurrent(Page) != null)
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "gt-apply", js, true);
        else
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "gt-apply", js, true);
    }

    protected void ddlLangTop_SelectedIndexChanged(object sender, EventArgs e)
    {
        var lang = ddlLangTop.SelectedValue ?? "es";
        SaveLangCookie(lang);
        RegisterApplyLanguageScript(lang);
        Response.Redirect(Request.RawUrl, endResponse: false);
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
    private string[] CurrentUserRoles()
    {
        var auth = Session["auth"] as UserSession;
        return (auth != null && auth.Roles != null) ? auth.Roles : new string[0];
    }

    private void BindSuggestions(string q)
    {
        q = (q ?? "").Trim();

        var roles = CurrentUserRoles();
        var data = PageDirectory.Search(q, roles)
                                .Select(p => new
                                {
                                    Title = p.Title,
                                    Url = ResolveUrl(p.Url) // "~/" -> "/..."
                                })
                                .ToList();

        repSuggest.DataSource = data;
        repSuggest.DataBind();
        pnlSuggest.Visible = data.Count > 0 && q.Length > 0;
        upSearch.Update();
    }

    // ====== eventos ======
    protected void txtTopSearch_TextChanged(object sender, EventArgs e)
    {
        BindSuggestions(txtTopSearch.Text);
    }

    protected void repSuggest_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "go")
        {
            var url = Convert.ToString(e.CommandArgument);
            if (!string.IsNullOrEmpty(url))
            {
                // cerramos el panel y redirigimos
                pnlSuggest.Visible = false;
                upSearch.Update();
                Response.Redirect(url, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }

}
