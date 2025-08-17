// Login.aspx.cs
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using BLL;
using BE;

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
                var bll = new BLLUsuario();

                BEUsuario be = bll.Login(email, password);      // devuelve null si credenciales inválidas

                if (be == null)
                {
                    lblLoginResult.Text = "Credenciales inválidas";
                    return;
                }

                if (!be.EmailVerified)
                {
                    Response.Redirect(ResolveUrl("/VerifyEmailPending.aspx"));
                    return;
                }


                Session["auth"] = new UserSession
                {
                    UserId = be.Id,
                    Email = be.Email,
                    EmailVerified = be.EmailVerified,
                    Roles = be.Roles ?? new string[0]
                };
                bll.RegistrarAcceso(be, Request.UserAgent);

                var target = !string.IsNullOrWhiteSpace(ret) && IsSafeLocalUrl(ret)
                    ? ret
                    : ResolveUrl("/Landing.aspx");

                Response.Redirect(target, endResponse: true);
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

