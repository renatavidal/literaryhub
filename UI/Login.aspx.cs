// Login.aspx.cs
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;

public partial class Login : PublicPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var auth = Session["auth"] as UserSession;
            if (auth != null)
                Response.Redirect(ResolveUrl("/Landing.aspx"));
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        var email = (txtEmail.Text ?? "").Trim();
        var password = txtPassword.Text ?? "";
        var remember = chkRemember.Checked;
        var ret = Request.QueryString["returnUrl"] ?? "";

        try
        {
            var baseUri = new Uri(Request.Url.GetLeftPart(UriPartial.Authority));
            var apiUri = new Uri(baseUri, ResolveUrl("/api/auth/login"));

            var dto = new { Email = email, Password = password, RememberMe = remember, ReturnUrl = ret };
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var handler = new HttpClientHandler { UseCookies = false })
            using (var http = new HttpClient(handler))
            {
                var resp = http.PostAsync(apiUri, content).Result;
                var body = resp.Content.ReadAsStringAsync().Result;

                if (!resp.IsSuccessStatusCode)
                {
                    lblLoginResult.Text = resp.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ? "Credenciales inválidas"
                        : ("Error de autenticación (" + (int)resp.StatusCode + ")");
                    return;
                }

                // Re-emitir las cookies que la API setea 
                if (resp.Headers.TryGetValues("Set-Cookie", out var setCookies))
                {
                    foreach (var sc in setCookies)
                        Response.AppendHeader("Set-Cookie", sc);
                }

                // Leer decisión de redirección del backend
                var data = JsonConvert.DeserializeObject<LoginResponse>(body) ?? new LoginResponse();
                var target = string.IsNullOrWhiteSpace(data.redirectUrl) ? ResolveUrl("/Landing.aspx") : data.redirectUrl;

                if (!IsSafeLocalUrl(target)) target = ResolveUrl("/Landing.aspx");

                Response.Redirect(target, endResponse: true);
            }
        }
        catch (Exception)
        {
            lblLoginResult.Text = "Hubo un problema al iniciar sesión. Probá de nuevo.";
        }
    }
    private bool IsSafeLocalUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        url = HttpUtility.UrlDecode(url).Trim();
        if (url.StartsWith("//") || url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return false;
        if (url.IndexOf('\\') >= 0 || url.IndexOf('\r') >= 0 || url.IndexOf('\n') >= 0) return false;

        if (VirtualPathUtility.IsAppRelative(url)) return true;

        var appPath = Request.ApplicationPath;
        if (!appPath.EndsWith("/")) appPath += "/";
        return url.StartsWith(appPath, StringComparison.OrdinalIgnoreCase) || url.StartsWith("/");
    }
    private class LoginResponse
    {
        public bool ok { get; set; }
        public string reason { get; set; }
        public string redirectUrl { get; set; }
    }

}
