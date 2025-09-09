// Login.aspx.cs
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using BLL;
using BE;

    public partial class Login : PublicPage
    {
        protected override bool RequireLogin
        {
            get { return false; }
        }

        protected override bool RequireVerifiedEmail
        {
            get { return false; }
        }
        protected override string[] RequiredRoles { get { return null; } }
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
            var ret = Request.QueryString["returnUrl"] ?? "";

            try
            {
                var bll = new BLLUsuario();

                var p = bll.LoginAny(email, password);


                if (p == null)
                {
                    lblLoginResult.Text = (GetLocalResourceObject("Login_InvalidCreds") as string) ?? "Credenciales inválidas";
                    return;
                }
            if (p.Activo == false)
            {
                lblLoginResult.Text = (GetLocalResourceObject("Login_UserDeactivated") as string) ?? "Usuario DESACTIVADO, si considerás que esto fue un error contactenos en literary.hub.contact@gmail.com";
                return;
            }
                Session["auth"] = new UserSession
                {
                    UserId = p.Id,
                    Email = p.Email,
                    EmailVerified = p.EmailVerified,
                    Roles = p.Roles ?? new string[0]
                };
                bll.RegistrarAcceso(p, Request.UserAgent);

                if (!p.EmailVerified)
                {
                    Response.Redirect("/VerifyEmailPending.aspx", endResponse: false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
               





            var target = !string.IsNullOrWhiteSpace(ret) && IsSafeLocalUrl(ret)
                    ? ret
                    : ResolveUrl("/Home.aspx");

                Response.Redirect(target, endResponse: false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception)
            {
                lblLoginResult.Text = (GetLocalResourceObject("Login_GenericError") as string) ?? "Hubo un problema al iniciar sesión. Probá de nuevo.";
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


