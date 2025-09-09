using System;
using System.Web.UI;

public partial class Purchase : ReaderPage
{
    protected override bool RequireLogin { get { return true; } }
    protected override bool RequireVerifiedEmail { get { return true; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        var auth = Session["auth"] as UserSession;
        if (auth.IsInRole("Client"))
        {
            var msg = (GetLocalResourceObject("Purch_ClientAlert") as string) ?? "Como cliente no puedes comprar libros, crea una cuenta como usuario para disfrutar de la experiencia.";
            ScriptManager.RegisterStartupScript(this, GetType(), "err", "alert('" + msg.Replace("'", "\\'") + "');", true);
            Response.Redirect("/Home.aspx");
        }
    }
}
