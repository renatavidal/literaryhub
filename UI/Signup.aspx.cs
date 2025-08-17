// Signup.aspx.cs
using System;
using System.Web.UI;

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

        // TODO: 1) Crear usuario como "no verificado" en tu DB (password con hash+salt).
        //       2) Generar token aleatorio y fecha de expiración.
        //       3) Enviar email con link: https://tu-dominio/VerifyEmail.aspx?token=XYZ
        // Ejemplo (pseudo):
        // var token = TokenService.CreateForEmail(txtEmail.Text);
        // EmailService.SendVerification(txtEmail.Text, token);

        lblSignupResult.CssClass = "success";
        lblSignupResult.Text = "¡Listo! Te enviamos un correo para verificar tu cuenta.";
    }
}
