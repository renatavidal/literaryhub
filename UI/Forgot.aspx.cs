using System;
using System.Configuration;
using BLL;
using Servicios;

public partial class Forgot : System.Web.UI.Page
{
    private readonly BLLUsuario _bll = new BLLUsuario();
    private bool token_correcto = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        SetPlaceholders();
        if (!IsPostBack)
        {
            string token = Request.QueryString["token"];
            if (!string.IsNullOrEmpty(token))
            {
                int userId;
                token_correcto = _bll.VerificarEmailPorToken(token, out userId);
                if (token_correcto)
                {
                    hidUserId.Value = userId.ToString();
                    hidCodeId.Value = token;

                    // Mostrar directamente paso 2
                    pnlStep1.Visible = false;
                    pnlStep2.Visible = true;
                    lblMsg2.CssClass = "msg ok";
                    lblMsg2.Text = (GetLocalResourceObject("Forgot_Step2Prompt") as string) ?? "Ingresá tu nueva contraseña.";
                }
                else
                {
                    pnlStep1.Visible = true;
                    pnlStep2.Visible = false;
                    lblMsg1.CssClass = "msg err";
                    lblMsg1.Text = (GetLocalResourceObject("Forgot_InvalidLink") as string) ?? "El enlace no es válido o ya expiró.";
                }
            }
        }
    }

    protected void btnSendCode_Click(object sender, EventArgs e)
    {
        string email = (txtEmail.Text ?? "").Trim();
        if (email.Length == 0)
        {
            lblMsg1.CssClass = "msg err";
            lblMsg1.Text = (GetLocalResourceObject("Forgot_EmailRequired") as string) ?? "Ingresá tu email.";
            return;
        }

        var user = _bll.ObtenerPorEmail(email); 
        lblMsg1.CssClass = "msg ok";
        lblMsg1.Text = (GetLocalResourceObject("Forgot_EmailSent") as string) ?? "Si el email existe, te enviamos un link para cambiar tu contraseña";

        if (user == null) return;

        string code = _bll.GenerarTokenVerificacion(user.Id);
        string baseUrl = ConfigurationManager.AppSettings["AppBaseUrl"];
        string verifyUrl = baseUrl + "/Forgot.aspx?token=" + Uri.EscapeDataString(code);
        _bll.EnviarVerificacionContrasena(user.Email, verifyUrl);

     
    }
 

    protected void btnReset_Click(object sender, EventArgs e)
    {
        int userId;

        if (!int.TryParse(hidUserId.Value, out userId))
        {
            lblMsg2.CssClass = "msg err";
            lblMsg2.Text = (GetLocalResourceObject("Forgot_InvalidSession") as string) ?? "Sesión de recuperación inválida. Reintentá el proceso.";
            return;
        }

        if ((txtNewPass.Text ?? "") != (txtNewPass2.Text ?? ""))
        {
            lblMsg2.CssClass = "msg err";
            lblMsg2.Text = (GetLocalResourceObject("Forgot_PasswordsMismatch") as string) ?? "Las contraseñas no coinciden.";
            return;
        }

        // Cambiar clave y marcar token como usado
        _bll.CambiarPassword(userId, txtNewPass.Text);


        lblMsg2.CssClass = "msg ok";
        lblMsg2.Text = "¡Contraseña actualizada!  Serás redirigido a el Log in en 5 segundos...";
        Response.AppendHeader("Refresh", "5;url=/Login.aspx");
        Context.ApplicationInstance.CompleteRequest();
        return;
    }
    void SetPlaceholders()
    {
        var phEmail = GetLocalResourceObject("Forgot_EmailPlaceholder") as string;
        if (!string.IsNullOrEmpty(phEmail)) txtEmail.Attributes["placeholder"] = phEmail;
        var ph1 = GetLocalResourceObject("Forgot_NewPwdPlaceholder") as string;
        if (!string.IsNullOrEmpty(ph1)) txtNewPass.Attributes["placeholder"] = ph1;
        var ph2 = GetLocalResourceObject("Forgot_ConfirmPlaceholder") as string;
        if (!string.IsNullOrEmpty(ph2)) txtNewPass2.Attributes["placeholder"] = ph2;
    }

}

