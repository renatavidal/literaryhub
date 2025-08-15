// Contact.aspx.cs
using System;
using System.Web.UI;

public partial class Contact : Page
{
    protected void Page_Load(object sender, EventArgs e) { }

    protected void btnSend_Click(object sender, EventArgs e)
    {
            lblResult.Text = "¡Gracias! Recibimos tu mensaje y te responderemos a la brevedad.";
            // Aquí podrías enviar un correo o guardar en DB.
    }
}
