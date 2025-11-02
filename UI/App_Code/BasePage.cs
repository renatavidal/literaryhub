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
public abstract class ReaderPage : AuthenticatedPage
{ 
    protected override bool RequireLogin { get { return true; } } 
}  


// =======================================
// =====  PÁGINAS CON PERMISOS  =========
// =======================================

// 1) Administrar usuarios
public abstract class Perm_AdminUsuariosPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Administrar usuarios" }; } }
}

// 2) Administrar suscripciones
public abstract class Perm_AdminSuscripcionesPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Administrar suscripciones" }; } }
}

// 3) Administrar encuestas
public abstract class Perm_AdminEncuestasPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Administrar encuestas" }; } }
}

// 4) Ver reportes
public abstract class Perm_VerReportesPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Ver reportes" }; } }
}

// 5) Administrar finanzas
public abstract class Perm_AdminFinanzasPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Administrar finanzas" }; } }
}

// 6) Administrar publicidades
public abstract class Perm_AdminPublicidadesPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Administrar publicidades" }; } }
}

// 7) Soporte / Chat
public abstract class Perm_SoporteChatPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Soporte/Chat" }; } }
}

// 8) Backups y restore
public abstract class Perm_BackupRestorePage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Backups y restore" }; } }
}

// 9) Bitácora administrativa
public abstract class Perm_AdminBitacoraPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "AdminBitacora" }; } }
}

// 10) Administrar Newsletter
public abstract class Perm_AdminNewsletterPage : VerifiedUserPage
{
    protected override string[] RequiredRoles { get { return new[] { "Administrar Newsletter" }; } }
}
