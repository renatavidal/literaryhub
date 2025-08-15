// Login.aspx.cs
using System;
using System.Web.UI;

public partial class Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Si desactivaste unobtrusive en Web.config, no hace falta nada acá.
        // this.UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        // TODO: Autenticar contra tu backend (hash + salt, etc.)
        // var user = AuthService.Login(txtEmail.Text, txtPassword.Text);
        // if (user == null) { lblLoginResult.Text = "Credenciales inválidas"; return; }
        // if (!user.EmailVerified) { lblLoginResult.Text = "Verificá tu email para activar la cuenta."; return; }
        // FormsAuthentication.SetAuthCookie(user.Email, chkRemember.Checked);
        // Response.Redirect("~/Landing.aspx");

        lblLoginResult.CssClass = "success";
        lblLoginResult.Text = "Demo: acá iría la autenticación y el redirect.";
    }
}
