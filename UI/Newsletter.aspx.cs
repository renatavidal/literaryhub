using System;
using BLL;

public partial class Newsletter : ReaderPage
{
    private const int PageSize = 10;
    private readonly BLLNewsletter _bll = new BLLNewsletter();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        if (!IsPostBack)
        {
            var auth = Session["auth"] as UserSession;
            if (auth != null && !string.IsNullOrWhiteSpace(auth.Email))
                txtEmail.Text = auth.Email;
        }

        int pageIndex = 0; int tmp;
        if (int.TryParse(Request["p"], out tmp) && tmp >= 0) pageIndex = tmp;

        var bll = new BLLNewsletter();
        int total;
        var list = bll.ListarPublicados(pageIndex, PageSize, out total);

        rptNews.DataSource = list;
        rptNews.DataBind();

        lnkPrev.Visible = pageIndex > 0;
        if (lnkPrev.Visible) lnkPrev.NavigateUrl = "?p=" + (pageIndex - 1);

        bool hasMore = (pageIndex + 1) * PageSize < total;
        lnkNext.Visible = hasMore;
        if (hasMore) lnkNext.NavigateUrl = "?p=" + (pageIndex + 1);

        litTotal.Text = "Total: " +  total.ToString();
    }
    private string GetEmailFromPageOrSession()
    {
        var email = (txtEmail.Text ?? "").Trim();
        if (!string.IsNullOrEmpty(email)) return email;

        var auth = Session["auth"] as UserSession;
        if (auth != null && !string.IsNullOrWhiteSpace(auth.Email)) return auth.Email;

        return "";
    }

    private int? GetUserIdOrNull()
    {
        var auth = Session["auth"] as UserSession;
        return (auth != null && auth.UserId > 0) ? (int?)auth.UserId : null;
    }

    private void TogglePanelsByStatus()
    {
        var email = GetEmailFromPageOrSession();
        bool subscribed = _bll.IsSubscribed(GetUserIdOrNull(), email);

        pnlForm.Visible = !subscribed;
        pnlOk.Visible = subscribed;

        // si está logueado y no hay email tipeado, prefill
        if (!IsPostBack && string.IsNullOrEmpty(txtEmail.Text))
        {
            var auth = Session["auth"] as UserSession;
            if (auth != null && !string.IsNullOrWhiteSpace(auth.Email))
                txtEmail.Text = auth.Email;
        }
    }
    protected void valCats_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        args.IsValid = chkReco.Checked || chkLaunch.Checked || chkEvtPres.Checked || chkEvtVirt.Checked;
    }

    protected void btnSubscribe_Click(object sender, EventArgs e)
    {
        Page.Validate();
        if (!Page.IsValid) return;

        try
        {
            _bll.Subscribe(
                GetUserIdOrNull(),
                (txtEmail.Text ?? "").Trim(),
                chkReco.Checked, chkLaunch.Checked, chkEvtPres.Checked, chkEvtVirt.Checked);

            lblResult.CssClass = "meta";
            lblResult.Text = "✅ ¡Listo! Te suscribimos al newsletter.";
        }
        catch (Exception ex)
        {
            lblResult.CssClass = "meta";
            lblResult.Text = "❌ Error: " + Server.HtmlEncode(ex.Message);
        }

        // después de suscribir, pasar a estado “ok”
        TogglePanelsByStatus();
    }

    protected void btnUnsub_Click(object sender, EventArgs e)
    {
        try
        {
            _bll.Unsubscribe(GetUserIdOrNull(), GetEmailFromPageOrSession());

            lblUnsub.CssClass = "meta";
            lblUnsub.Text = "🗑️ Suscripción cancelada.";
        }
        catch (Exception ex)
        {
            lblUnsub.CssClass = "meta";
            lblUnsub.Text = "❌ " + Server.HtmlEncode(ex.Message);
        }

        // vuelve a mostrar el formulario por si quiere re-suscribirse
        TogglePanelsByStatus();
    }

}
