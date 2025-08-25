// Contact.aspx.cs
using BE;
using BLL;
using System;
using System.Web.UI;

public partial class Contact : PublicPage
{
    protected void Page_Load(object sender, EventArgs e) { }

    protected void btnSend_Click(object sender, EventArgs e)
    {


        var email = (txtEmail.Text ?? "").Trim();
        var text = txtMessage.Text ?? "";

        try
        {
            var bll = new BLLUsuario();
            var userId = 0;
            if (CurrentUser != null)
            {
                 userId = CurrentUser.UserId;
            }

            bll.Contacto(email, text, userId);
            lblResult.Text = "¡Gracias! Recibimos tu mensaje y te responderemos a la brevedad.";


        }
        catch (Exception)
        {
            lblResult.Text = "Hubo un problema al iniciar sesión. Probá de nuevo.";
        }
    }
}