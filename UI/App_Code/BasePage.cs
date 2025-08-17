using System;

public abstract class AuthPage : System.Web.UI.Page
{
    // Configurables por las clases hijas (sin '=>' ni null-prop)
    protected virtual bool RequireLogin { get { return true; } }
    protected virtual bool RequireVerifiedEmail { get { return false; } }
    protected virtual string[] RequiredRoles { get { return new string[0]; } }
    protected virtual string LoginUrl { get { return "/Login.aspx"; } }
    protected virtual string AccessDeniedUrl { get { return "/AccessDenied.aspx"; } }
    protected virtual string VerifyEmailUrl { get { return "/VerifyEmailPending.aspx"; } }

    // Sesión actual (sin '?.')
    protected UserSession CurrentUser
    {
        get
        {
            return (Session != null) ? (Session["auth"] as UserSession) : null;
        }
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        var auth = CurrentUser;

        if (RequireLogin && auth == null)
        {
            RedirectToLogin();
            return;
        }

        if (RequireLogin && RequireVerifiedEmail && auth != null && !auth.EmailVerified)
        {
            Response.Redirect(ResolveUrl(VerifyEmailUrl), true);
            return;
        }

        if (RequireLogin && RequiredRoles != null && RequiredRoles.Length > 0)
        {
            bool ok = (auth != null) && HasAnyRole(auth, RequiredRoles);
            if (!ok)
            {
                Response.Redirect(ResolveUrl(AccessDeniedUrl), true);
                return;
            }
        }
    }

    protected void RedirectToLogin()
    {
        string ret = (Request != null && Request.RawUrl != null) ? Request.RawUrl : "/";
        string url = ResolveUrl(LoginUrl) + "?returnUrl=" + Server.UrlEncode(ret);
        Response.Redirect(url, true);
    }

    private static bool HasAnyRole(UserSession auth, string[] required)
    {
        if (auth == null || auth.Roles == null || required == null) return false;

        for (int i = 0; i < required.Length; i++)
        {
            string r = required[i];
            if (auth.IsInRole(r)) return true;
        }
        return false;
    }
}

// Páginas públicas (no requieren login)
public abstract class PublicPage : AuthPage
{
    protected override bool RequireLogin { get { return false; } }
}

// Cualquier usuario autenticado
public abstract class AuthenticatedPage : AuthPage
{
    // RequireLogin = true (de la base)
}

// Autenticado + email verificado
public abstract class VerifiedUserPage : AuthenticatedPage
{
    protected override bool RequireVerifiedEmail { get { return true; } }
}

// Roles específicos
public abstract class AdminPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Admin" }; } }
}

public abstract class AdminOrEditorPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Admin", "Editor" }; } }
}

public abstract class ReaderPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Reader" }; } }
}
