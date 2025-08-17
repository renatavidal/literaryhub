using System;
using System.Web.UI;

// Si usás las páginas base centralizadas, descomentá la línea de abajo y
// cambiá Page por AuthenticatedPage en la herencia.
// public partial class VerifyEmailPending : AuthenticatedPage
public partial class VerifyEmailPending : ReaderPage
{
    protected UserSession CurrentUser => Session["auth"] as UserSession;

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

        // Mostrar email enmascarado
        litMaskedEmail.Text = MaskEmail(CurrentUser.Email);
    }

    protected void btnResend_Click(object sender, EventArgs e)
    {
        try
        {
            // TODO: Generar y guardar nuevo token de verificación en DB (expira en X horas)
            // var token = TokenService.CreateForEmail(CurrentUser.UserId);
            // EmailService.SendVerification(CurrentUser.Email, token);

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
        // TODO: Refrescar estado desde tu DB
        // var verified = UserRepository.IsEmailVerified(CurrentUser.UserId);

        bool verified = false; // DEMO: reemplazar por valor real
        if (verified)
        {
            // Actualizar sesión y redirigir
            CurrentUser.EmailVerified = true;
            lblStatus.CssClass = "ok";
            lblStatus.Text = "¡Tu email ya está verificado! Redirigiendo...";

            // Si venías con returnUrl, respetalo
            var ret = Request.QueryString["returnUrl"];
            Response.Redirect(string.IsNullOrEmpty(ret) ? "/Landing.aspx" : ret);
        }
        else
        {
            lblStatus.CssClass = "err";
            lblStatus.Text = "Aún no vemos la verificación. Hacé clic en el enlace del correo o reintentá en unos segundos.";
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
