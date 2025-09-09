// Signup.aspx.cs
using System;
using System.Web.UI;
using BLL;
using BE;
using Servicios;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Xml.Linq;
using System.Web;
using System.Text.Json.Serialization;

public partial class Signup : PublicPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // this.UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
    }

    protected void cvTerms_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        args.IsValid = chkTerms.Checked;
    }

    protected void btnSignup_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        string name = (txtName.Text ?? "").Trim();
        string lastName = (txtLastName.Text ?? "").Trim();
        string email = (txtEmail.Text ?? "").Trim();
        string password = txtPassword.Text ?? "";
        if (!ValidateRecaptcha())
        {
            lblSignupResult.CssClass = "error";
            lblSignupResult.Text = (GetLocalResourceObject("Signup_Captcha_Check") as string) ?? "Verificá el captcha.";
            return;
        }
        var svc = new EmailExists();
        bool disponible = svc.EmailDisponible(email);
        if (!disponible)
        {
            lblSignupResult.CssClass = "error";
            lblSignupResult.Text = (GetLocalResourceObject("Signup_Email_Taken") as string) ?? "Ese email ya está registrado.";
            return;
        }
        try
        {
            BLLUsuario bll = new BLLUsuario();
            int newUserId = bll.Registrar(email, name, lastName, password, false);

            lblSignupResult.CssClass = "success";
            lblSignupResult.Text = (GetLocalResourceObject("Signup_Success") as string) ?? "¡Listo! Te enviamos un correo para verificar tu cuenta.";

            Response.Redirect("/VerifyEmailPending.aspx?email=" + Server.UrlEncode(email) + "&tipo=usuario" +  "&id=" + newUserId, false);
        }
        catch (Exception)
        {
            lblSignupResult.CssClass = "error";
            lblSignupResult.Text = (GetLocalResourceObject("Signup_GenericError") as string) ?? "No pudimos crear tu cuenta. Intentá de nuevo.";
        }
        finally
        {
            // Seguridad: limpiamos los campos de password
            txtPassword.Text = "";
            txtConfirm.Text = "";
        }
    }
    public void cvCaptcha_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        args.IsValid = ValidateRecaptcha();
    }

    public bool ValidateRecaptcha()
    {
        try
        {
            var response = Request.Form["g-recaptcha-response"];
            if (string.IsNullOrEmpty(response)) return false;

            var secret = ConfigurationManager.AppSettings["RecaptchaSecret"];
            var remoteIp = Request.UserHostAddress; // opcional

            // (Opcional pero recomendado en .NET 4.7.x)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Armar URL sin interpolación y con URL-encoding
            var verifyUrl = string.Format(
                "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}{2}",
                HttpUtility.UrlEncode(secret),
                HttpUtility.UrlEncode(response),
                string.IsNullOrEmpty(remoteIp) ? "" : "&remoteip=" + HttpUtility.UrlEncode(remoteIp)
            );

            using (var client = new WebClient())
            {
                var json = client.DownloadString(verifyUrl);
                dynamic obj = JsonConvert.DeserializeObject(json);
                return obj.success == true;
            }
        }
        catch
        {
            return false; // ante error de verificación, tratá como inválido
        }
    }
}



