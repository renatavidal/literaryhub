using System;
using System.Globalization;
using System.Threading;
using System.Web;

public abstract class AuthPage : System.Web.UI.Page
{
    
    protected virtual bool RequireLogin { get { return true; } }
    protected virtual bool RequireVerifiedEmail { get { return false; } }
    protected virtual string[] RequiredRoles { get { return new string[0]; } }
    protected virtual string LoginUrl { get { return "/Login.aspx"; } }
    protected virtual string AccessDeniedUrl { get { return "/AccessDenied.aspx"; } }
    protected virtual string VerifyEmailUrl { get { return "/VerifyEmailPending.aspx"; } }

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
        var here = VirtualPathUtility.ToAppRelative(Request.Path).ToLowerInvariant();
        string verifyUrlAppRel = VirtualPathUtility
                                    .ToAppRelative(ResolveUrl(VerifyEmailUrl))
                                    .ToLowerInvariant();
        string loginUrlAppRel = VirtualPathUtility
                                    .ToAppRelative(ResolveUrl(LoginUrl))
                                    .ToLowerInvariant();
        if (RequireLogin && auth == null)
        {
            if (here != loginUrlAppRel)
                RedirectToLogin();
            return;
        }

        if (RequireLogin && RequireVerifiedEmail && auth != null && !auth.EmailVerified)
        {
            if (here != verifyUrlAppRel)
                SafeRedirect(ResolveUrl(VerifyEmailUrl));
            return;
        }

        if (RequireLogin && RequiredRoles != null && RequiredRoles.Length > 0)
        {
            bool ok = (auth != null) && HasAnyRole(auth, RequiredRoles);
            if (!ok)
            {
                SafeRedirect(ResolveUrl(AccessDeniedUrl));
                return;
            }
        }
    }
    protected void SafeRedirect(string url)
    {
        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void RedirectToLogin()
    {
        string ret = (Request != null && Request.RawUrl != null) ? Request.RawUrl : "/";
        string url = ResolveUrl(LoginUrl) + "?returnUrl=" + Server.UrlEncode(ret);
        SafeRedirect(url);
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
    protected override string[] RequiredRoles { get { return new[] { "Reader", "Admin", "Editor" }; } }
}
