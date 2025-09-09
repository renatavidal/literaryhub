
using System;
using BE;
using BLL;

public partial class PurchaseConfirmation : ReaderPage
{
    protected override bool RequireLogin { get { return true; } }
    protected override bool RequireVerifiedEmail { get { return true; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        int id;
        if (!int.TryParse(Request["id"], out id) || id <= 0)
        {
            Response.Redirect("~/Home.aspx");
            return;
        }
        litId.Text = id.ToString();

        var bll = new BLLCatalog();
        var det = bll.GetPurchaseDetails(id);

        if (det != null)
        {
            litTitle.Text = Server.HtmlEncode(det.Title);
            litAuthors.Text = Server.HtmlEncode(det.Authors);
            litAmount.Text = (det.Currency ?? "USD") + " " + det.Price.ToString("0.00");
            litStatus.Text = StatusText(det.Status);
            litWhen.Text = ((GetLocalResourceObject("PC_CreatedPrefix") as string) ?? "Creada el ") + det.CreatedUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
            boxDetails.Visible = true;
        }
        else
        {
            // compra inexistente o sin detalles
            boxDetails.Visible = false;
        }
    }

    private string StatusText(byte s)
    {
        return s == 1 ? ((GetLocalResourceObject("PC_Status_Paid") as string) ?? "Pagada") : (s == 2 ? ((GetLocalResourceObject("PC_Status_Failed") as string) ?? "Fallida") : ((GetLocalResourceObject("PC_Status_Pending") as string) ?? "Pendiente"));
    }
}

