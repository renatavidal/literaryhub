using System;
using System.Configuration;
using System.Web.UI;
using BLL;

// Si usás las páginas base centralizadas, descomentá la línea de abajo y
// cambiá Page por AuthenticatedPage en la herencia.
// public partial class VerifyEmailPending : AuthenticatedPage
public partial class VerifyEmailPending : ReaderPage
{
    protected override bool RequireLogin
    {
        get { return true; }
    }

    protected override bool RequireVerifiedEmail
    {
        get { return false; }
    }
    protected override string[] RequiredRoles { get { return null; } }
    protected UserSession CurrentUser
    {
        get
        {
            return (Session != null) ? (Session["auth"] as UserSession) : null;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Debe estar logueado para ver esta página
        if (CurrentUser == null)
        {
            //este returnurl hace que despues de loguearse el usuario vuelva a esta pagina a verificar el mail
            Response.Redirect("/Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
            return;
        }

        // Si YA está verificado, no tiene sentido estar acá
        if (CurrentUser.EmailVerified)
        {
            Response.Redirect("/Landing.aspx");
            return;
        }
        var bll = new BLL.BLLUsuario();
        string token_url = Request.QueryString["token"];
        if (!string.IsNullOrEmpty(token_url))
        {
            int userId;
            bool ok = bll.VerificarEmailPorToken(token_url, out userId);

            if (ok && userId == CurrentUser.UserId)
            {
                CurrentUser.EmailVerified = true;
                Session["auth"] = CurrentUser;

                lblStatus.CssClass = "ok";
                lblStatus.Text = "¡Tu email fue verificado!";
                Response.Redirect("/Home.aspx");
            }
            else
            {
                lblStatus.CssClass = "err";
                lblStatus.Text = "Enlace inválido o vencido.";
            }
        }
        else{
            enviar_email();
        }
       
        


    }
    public void enviar_email()
    {
        var bll = new BLL.BLLUsuario();
        var userId = CurrentUser.UserId;
        string token = bll.GenerarTokenVerificacion(userId);
        string baseUrl = ConfigurationManager.AppSettings["AppBaseUrl"];
        string verifyUrl = baseUrl + "/VerifyEmailPending.aspx?token=" + Uri.EscapeDataString(token);
        string email = CurrentUser.Email;

        bll.EnviarVerificacion(email, verifyUrl);
        string masked = MaskEmail(CurrentUser.Email ?? "");
        litMaskedEmail.Text = Server.HtmlEncode(masked);
    }
    protected void btnResend_Click(object sender, EventArgs e)
    {
        try
        {
            enviar_email();

            lblStatus.CssClass = "ok";
            lblStatus.Text = "Listo. Te reenviamos el enlace de verificación. Revisá tu bandeja de entrada.";
        }
        catch (Exception)
        {
            lblStatus.CssClass = "err";
            lblStatus.Text = "No pudimos reenviar el correo ahora. Probá nuevamente en unos minutos.";
        }
    }

    protected void btnCheck_Click(object sender, EventArgs e)
    {
        var bll = new BLL.BLLUsuario();
        string token_url = Request.QueryString["token"];
        if (!string.IsNullOrEmpty(token_url))
        {
            int userId;
            bool ok = bll.VerificarEmailPorToken(token_url, out userId);

            if (ok && userId == CurrentUser.UserId)
            {
                CurrentUser.EmailVerified = true;
                Session["auth"] = CurrentUser;

                lblStatus.CssClass = "ok";
                lblStatus.Text = "¡Tu email fue verificado!";
                Response.Redirect("/Home.aspx");
            }
            else
            {
                lblStatus.CssClass = "err";
                lblStatus.Text = "Enlace inválido o vencido.";
            }
        }
    }

    private static string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@")) return email ?? "";
        var parts = email.Split('@');
        var user = parts[0];
        var domain = parts[1];

        string maskUser = user.Length <= 2
            ? new string('*', user.Length)
            : user.Substring(0, 2) + new string('*', Math.Max(0, user.Length - 2));

     
        var dot = domain.IndexOf('.');
        if (dot <= 1) return maskUser + "@" + domain; 
        var domName = domain.Substring(0, dot);
        var tld = domain.Substring(dot);
        string maskDom = domName.Length <= 2
            ? new string('*', domName.Length)
            : domName.Substring(0, 1) + new string('*', Math.Max(0, domName.Length - 2)) + domName.Substring(domName.Length - 1, 1);

        return maskUser + "@" + maskDom + tld;
    }
}
