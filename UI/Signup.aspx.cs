// Signup.aspx.cs
using System;
using System.Web.UI;
using BLL;
using BE;
using Servicios;

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

        try
        {
            BLLUsuario bll = new BLLUsuario();
            int newUserId = bll.Registrar(email, name, lastName, password, false); 

            lblSignupResult.CssClass = "success";
            lblSignupResult.Text = "¡Listo! Te enviamos un correo para verificar tu cuenta.";

            Response.Redirect("/VerifyEmailPending.aspx?email=" + Server.UrlEncode(email), false);
        }
        catch (Exception)
        {
            lblSignupResult.CssClass = "error";
            lblSignupResult.Text = "No pudimos crear tu cuenta. Intentá de nuevo.";
        }
        finally
        {
            // Seguridad: limpiamos los campos de password
            txtPassword.Text = "";
            txtConfirm.Text = "";
        }
    }
}

