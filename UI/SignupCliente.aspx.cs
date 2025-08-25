using System;
using System.Configuration;
using System.Net;
using System.Web;
using BE;
using BLL;
using Newtonsoft.Json;

public partial class SignupCliente : System.Web.UI.Page
{
    private readonly BLLCliente _bll = new BLLCliente();

    protected void btnCrear_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        if (!ValidateRecaptcha()) // o copia tu validador aquí
        {
            lblResultado.CssClass = "error";
            lblResultado.Text = "Verificá el captcha.";
            return;
        }

        try
        {
            var c = new BECliente
            {
                Cuil = (txtCuil.Text ?? "").Trim(),
                Nombre = (txtNombre.Text ?? "").Trim(),
                Apellido = (txtApellido.Text ?? "").Trim(),
                NegocioAlias = (txtAlias.Text ?? "").Trim(),
                Email = (txtEmail.Text ?? "").Trim(),
                Telefono = (txtTel.Text ?? "").Trim(),
                Fac_CondIVA = ddlIVA.SelectedValue,
                Fac_RazonSocial = (txtRazSoc.Text ?? "").Trim(),
                Fac_Cuit = (txtCuitFac.Text ?? "").Trim(),
                Fac_Domicilio = (txtDomFac.Text ?? "").Trim(),
                Fac_Email = (txtEmailFac.Text ?? "").Trim(),
                Tipo = rblTipo.SelectedValue,
                Ubicacion = (rblTipo.SelectedValue == "LIB" ? (txtUbicacion.Text ?? "").Trim() : null)
            };


            int id = _bll.Registrar(c);
            lblResultado.CssClass = "success";
            lblResultado.Text = "Cliente creado. Id: " + id;
        }
        catch (Exception ex)
        {
            lblResultado.CssClass = "error";
            lblResultado.Text = ex.Message;
        }
    }
    protected void cvUbicacion_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        var tipo = rblTipo.SelectedValue ?? "AUT";
        args.IsValid = (tipo != "LIB") || !string.IsNullOrWhiteSpace(txtUbicacion.Text);
    }
    public bool ValidateRecaptcha()
    {
        try
        {
            var response = Request.Form["g-recaptcha-response"];
            if (string.IsNullOrEmpty(response)) return false;

            var secret = ConfigurationManager.AppSettings["RecaptchaSecretLocal"];
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
