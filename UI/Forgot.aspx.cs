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
                    lblMsg2.Text = "Ingresá tu nueva contraseña.";
                }
                else
                {
                    pnlStep1.Visible = true;
                    pnlStep2.Visible = false;
                    lblMsg1.CssClass = "msg err";
                    lblMsg1.Text = "El enlace no es válido o ya expiró.";
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
            lblMsg1.Text = "Ingresá tu email.";
            return;
        }

        var user = _bll.ObtenerPorEmail(email); 
        lblMsg1.CssClass = "msg ok";
        lblMsg1.Text = "Si el email existe, te enviamos un link para cambiar tu contraseña";

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
            lblMsg2.Text = "Sesión de recuperación inválida. Reintentá el proceso.";
            return;
        }

        if ((txtNewPass.Text ?? "") != (txtNewPass2.Text ?? ""))
        {
            lblMsg2.CssClass = "msg err";
            lblMsg2.Text = "Las contraseñas no coinciden.";
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
}
