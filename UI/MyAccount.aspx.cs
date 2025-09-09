using System;
using System.Web.UI.WebControls;
using BLL;

public partial class MyAccount : ReaderPage
{
    protected override bool RequireLogin { get { return true; } }
    protected override bool RequireVerifiedEmail { get { return true; } }

    private int CurrentUserId
    {
        get
        {
            var auth = Session["auth"] as UserSession;
            if (auth == null)
            {
                var ret = Server.UrlEncode(Request.RawUrl);
                Response.Redirect("/Login.aspx?returnUrl=" + ret);
                return 0;
            }
            return auth.UserId;
        }
    }
    private void BindBooks()
    {
        repWant.DataSource = BLLBooks.GetWantToRead(CurrentUserId);
        repWant.DataBind();

        repRead.DataSource = BLLBooks.GetRead(CurrentUserId);
        repRead.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindBooks();
        }
    }

    protected void repWant_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int bookId;
        if (!int.TryParse(Convert.ToString(e.CommandArgument), out bookId)) return;

        if (e.CommandName == "MoveToRead")
        {
            BLLBooks.MoveToRead(CurrentUserId, bookId);
        }
        else if (e.CommandName == "Remove")
        {
            BLLBooks.RemoveFromLists(CurrentUserId, bookId, "want");
        }
        BindBooks();
    }

    protected void repRead_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int bookId;
        if (!int.TryParse(Convert.ToString(e.CommandArgument), out bookId)) return;

        if (e.CommandName == "Remove")
        {
            BLLBooks.RemoveFromLists(CurrentUserId, bookId, "read");
        }
        BindBooks();
    }

    protected void btnGoToForgot_Click(object sender, EventArgs e)
    {
        Response.Redirect(ResolveUrl("/Forgot.aspx?mode=change"));
    }

    protected void btnDeactivate_Click(object sender, EventArgs e)
    {
        try
        {
                var bll = new BLLUsuario();
                bll.Deactivate(CurrentUserId);

            try
            {
                var auth = Session["auth"] as UserSession;
                var email = (auth != null) ? auth.Email : "(sin sesión)";
                var bit = new BLLBitacora();
                bit.Registrar(CurrentUserId, email, "Usuario dado de baja desde MyAccount");
            }
            catch { /* nunca frenar la baja por un error de bitácora */ }

            lblDeactivateMsg.Text = (GetLocalResourceObject("Acc_Deactivate_Success") as string) ?? "Tu cuenta fue desactivada.";

            // Opcional: cerrar sesión inmediatamente
            Session.Clear();
            Session.Abandon();
            Response.Redirect(ResolveUrl("~/Landing.aspx"));
        }
        catch (Exception ex)
        {
            lblDeactivateMsg.Text = ((GetLocalResourceObject("Acc_ErrorPrefix") as string) ?? "Error: ") + ex.Message;
        }
    }
}

