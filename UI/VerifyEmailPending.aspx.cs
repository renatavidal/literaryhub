using System;
using System.Configuration;
using System.Web.UI;
using BE;
using BLL;
using Newtonsoft.Json.Linq;

// Si usás las páginas base centralizadas, descomentá la línea de abajo y
// cambiá Page por AuthenticatedPage en la herencia.
// public partial class VerifyEmailPending : AuthenticatedPage
public partial class VerifyEmailPending : ReaderPage
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
    protected UserSession CurrentUser
    {
        get
        {
            return (Session != null) ? (Session["auth"] as UserSession) : null;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        
      
        string token_url = Request.QueryString["token"];
        if (!string.IsNullOrEmpty(token_url))
        {
            VerificarPorToken(token_url);
        }
        else
        {
            EnviarEmailVerificacion();
        }
        string email = "";
        if (CurrentUser == null)
        {
            email = Request.QueryString["email"];
        }
        else
        {
            email = CurrentUser.Email;
        }

        litMaskedEmail.Text = Server.HtmlEncode(MaskEmail(email));
        lblStatus.Text = (GetLocalResourceObject("Verify_Status_Sent") as string) ?? "Te enviamos un correo con el enlace de verificación.";




    }
    public bool EsCliente()
    {
        if (CurrentUser != null && CurrentUser.IsInRole("Cliente")) return true;
        var t = (Request.QueryString["tipo"] ?? "").Trim().ToLowerInvariant();
        return t == "cliente" || t == "client";
    }
    private void VerificarPorToken(string token)
    {
        bool ok; int userId;

        if (EsCliente())
        {
            var bll = new BLL.BLLCliente();
            ok = bll.VerificarEmailPorToken(token, out userId);
        }
        else
        {
            var bll = new BLL.BLLUsuario();
            ok = bll.VerificarEmailPorToken(token, out userId);
        }

        if (!ok)
        {
            lblStatus.CssClass = "err";
            lblStatus.Text = (GetLocalResourceObject("Verify_Status_Invalid") as string) ?? "Enlace inválido o vencido.";
            return;
        }
        
          

            lblStatus.CssClass = "ok";
            lblStatus.Text = (GetLocalResourceObject("Verify_Status_Verified") as string) ?? "¡Tu email fue verificado!";

            Response.Redirect("/Login.aspx", endResponse: false);
            Context.ApplicationInstance.CompleteRequest();
            return;

    }
    private void EnviarEmailVerificacion()
    {
        string email = "";
        int id = 0;
        if (CurrentUser == null)
        {
            email = Request.QueryString["email"];
            id = Convert.ToInt32( Request.QueryString["id"]);

        }
        else
        {
            email = CurrentUser.Email;
            id = CurrentUser.UserId;
        }
           
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException("Falta el email.");

        string token;

        if (EsCliente())
        {
            var bll = new BLL.BLLCliente();
            // Si no hay sesión, necesitás obtener el Id por email
            BEUsuarioAuth user = new BEUsuarioAuth();
            user = bll.ObtenerPorEmail(email); 
            id = user.Id;
            if (id <= 0) throw new InvalidOperationException("No encontramos ese cliente.");
            token = bll.GenerarTokenVerificacion(id);
            var verifyUrl = BuildVerifyUrl(token, "cliente", email , id.ToString());
            bll.EnviarVerificacion(email, verifyUrl);
        }
        else
        {
            var bll = new BLL.BLLUsuario();
            BEUsuario user =  bll.ObtenerPorEmail(email); 
            if (user != null)
            {
                id = user.Id;
                if (id <= 0) throw new InvalidOperationException("No encontramos ese usuario.");
                token = bll.GenerarTokenVerificacion(id);
                var verifyUrl = BuildVerifyUrl(token, "usuario", email,  id.ToString());
                bll.EnviarVerificacion(email, verifyUrl);
            }
           
        }

        litMaskedEmail.Text = Server.HtmlEncode(MaskEmail(email));
    }
    protected void btnResend_Click(object sender, EventArgs e)
    {
        try
        {
            EnviarEmailVerificacion();

            lblStatus.CssClass = "ok";
            lblStatus.Text = (GetLocalResourceObject("Verify_Status_Resent") as string) ?? "Listo. Te reenviamos el enlace de verificación. Revisá tu bandeja de entrada.";
        }
        catch (Exception)
        {
            lblStatus.CssClass = "err";
            lblStatus.Text = (GetLocalResourceObject("Verify_Status_ResendError") as string) ?? "No pudimos reenviar el correo ahora. Probá nuevamente en unos minutos.";
        }
    }
    private string BuildVerifyUrl(string token, string tipo, string email , string id)
    {
        string baseUrl = (ConfigurationManager.AppSettings["AppBaseUrl"] ?? "").TrimEnd('/');
        string tokenEnc = Uri.EscapeDataString(token ?? "");
        string tipoEnc = Uri.EscapeDataString(tipo ?? "");
        return string.Format("{0}/VerifyEmailPending.aspx?token={1}&tipo={2}&id={4}&email={3}",
                             baseUrl, tokenEnc, tipoEnc, email, id);
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




