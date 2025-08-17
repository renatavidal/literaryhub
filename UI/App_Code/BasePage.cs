using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;

public abstract class AuthPage : System.Web.UI.Page
{
    // Cambiables por clase hija si querés
    protected virtual bool RequireLogin => true;
    protected virtual bool RequireVerifiedEmail => false;
    protected virtual string[] RequiredRoles => new string[0];
    protected virtual string LoginUrl => "/Login.aspx";
    protected virtual string AccessDeniedUrl => "/AccessDenied.aspx";
    protected virtual string VerifyEmailUrl => "/VerifyEmailPending.aspx";

    protected UserSession CurrentUser => Context?.Session?["auth"] as UserSession;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Session["auth"] == null) TryPopulateSessionFromApi();

        var auth = CurrentUser;

        if (RequireLogin && auth == null)
        {
            var ret = Server.UrlEncode(Request.RawUrl);
            Response.Redirect(ResolveUrl(LoginUrl) + "?returnUrl=" + ret, true);
            return;
        }
        if (RequireLogin && RequireVerifiedEmail && auth != null && !auth.EmailVerified)
        {
            Response.Redirect(ResolveUrl(VerifyEmailUrl), true);
            return;
        }
        if (RequireLogin && RequiredRoles.Length > 0)
        {
            bool ok = auth != null && Array.Exists(RequiredRoles, r => auth.IsInRole(r));
            if (!ok) { Response.Redirect(ResolveUrl(AccessDeniedUrl), true); return; }
        }
    }
    private void TryPopulateSessionFromApi()
    {
        try
        {
            var cookieHeader = Request.Headers["Cookie"];
            if (string.IsNullOrEmpty(cookieHeader)) return;

            var baseUri = new Uri(Request.Url.GetLeftPart(UriPartial.Authority));
            var apiUri = new Uri(baseUri, ResolveUrl("/api/me"));

            using (var handler = new HttpClientHandler { UseCookies = false })
            using (var http = new HttpClient(handler))
            {
                var req = new HttpRequestMessage(HttpMethod.Get, apiUri);
                req.Headers.TryAddWithoutValidation("Cookie", cookieHeader);

                var resp = http.SendAsync(req).Result;
                if (!resp.IsSuccessStatusCode) return;

                var json = resp.Content.ReadAsStringAsync().Result;
                var me = JsonConvert.DeserializeObject<MeResponse>(json);

                if (me?.authenticated == true)
                {
                    Session["auth"] = new UserSession
                    {
                        UserId = me.userId,
                        Email = me.email,
                        EmailVerified = me.emailVerified,
                        Roles = me.roles ?? new string[0]
                    };
                }
            }
        }
        catch { /* si falla, la página redirigirá a Login en el flujo normal */ }
    }

    private class MeResponse
    {
        public bool authenticated { get; set; }
        public int userId { get; set; }
        public string email { get; set; }
        public bool emailVerified { get; set; }
        public string[] roles { get; set; }
    }
}


// Páginas públicas (no requieren login)
public abstract class PublicPage : AuthPage
{
    protected override bool RequireLogin => false;
}

// Cualquier usuario autenticado
public abstract class AuthenticatedPage : AuthPage
{
    // RequireLogin = true (de la base)
}

// Autenticado + email verificado (si usás verificación)
public abstract class VerifiedUserPage : AuthenticatedPage
{
    protected override bool RequireVerifiedEmail => true;
}

// Roles específicos
public abstract class AdminPage : VerifiedUserPage
{
    protected override string[] RequiredRoles => new[] { "Admin" };
}

public abstract class AdminOrEditorPage : VerifiedUserPage
{
    protected override string[] RequiredRoles => new[] { "Admin", "Editor" };
}

public abstract class ReaderPage : VerifiedUserPage
{
    protected override string[] RequiredRoles => new[] { "Reader" };
}
