using System;
using System.Globalization;
using System.Collections.Generic;
using BE;
using Servicios;
using System.Web.UI.WebControls;

public partial class Checkout : System.Web.UI.Page
{
    private BLL.BLLSubscription _bll = new BLL.BLLSubscription(new Servicios.SmtpEmailService());

    protected void Page_Load(object sender, EventArgs e)
    {
        outputmessage.Visible = false;
        if (!IsPostBack)
        {
            string code = Request.QueryString["plan"];
            var plan = _bll.GetPlanByCode(code);
            if (plan == null) { litMsg.Text = "<div class='hint'>Plan no encontrado.</div>"; btnPagar.Enabled = false; return; }

            txtPlan.Text = plan.Descripcion + " (" + plan.Codigo + ")";
            txtTotal.Text = plan.PrecioUSD.ToString("0.##", CultureInfo.InvariantCulture);
            hidPlanTotal.Value = plan.PrecioUSD.ToString(CultureInfo.InvariantCulture);

            paymentForm.Standalone = false;

            // por UX: prellenar con total
            if (string.IsNullOrWhiteSpace(txtCardAmount.Text))
                txtCardAmount.Text = hidPlanTotal.Value;
        }
    }

    private static bool TryParseAmount(string s, out decimal d)
    {
        s = (s ?? "").Trim().Replace(',', '.');
        return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out d);
    }
    protected void valAnyMethod_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (chkCard.Checked || chkNC.Checked || chkAccount.Checked);
    }

    protected void valCardAmt_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!chkCard.Checked) { args.IsValid = true; return; }
        decimal v; args.IsValid = (TryParseAmount(txtCardAmount.Text, out v) && v > 0m);
    }

    protected void valNcIdRequired_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!chkNC.Checked) { args.IsValid = true; return; }
        args.IsValid = !string.IsNullOrWhiteSpace(txtNcId.Text);
    }

    protected void valNcAmt_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!chkNC.Checked) { args.IsValid = true; return; }
        decimal v; args.IsValid = (TryParseAmount(txtNcAmount.Text, out v) && v > 0m);
    }

    protected void valAccAmt_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!chkAccount.Checked) { args.IsValid = true; return; }
        decimal v; args.IsValid = (TryParseAmount(txtAccountAmount.Text, out v) && v > 0m);
    }

    protected void valTotals_ServerValidate(object source, ServerValidateEventArgs args)
    {
        decimal totalPlan; TryParseAmount(hidPlanTotal.Value, out totalPlan);

        decimal aCard = 0m, aNc = 0m, aAcc = 0m;
        if (chkCard.Checked) TryParseAmount(txtCardAmount.Text, out aCard);
        if (chkNC.Checked) TryParseAmount(txtNcAmount.Text, out aNc);
        if (chkAccount.Checked) TryParseAmount(txtAccountAmount.Text, out aAcc);

        args.IsValid = ((aCard + aNc + aAcc) == totalPlan);
    }

    protected void valNcFunds_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!chkNC.Checked) { args.IsValid = true; return; }

        decimal req;
        if (string.IsNullOrWhiteSpace(txtNcId.Text) ||
            !TryParseAmount(txtNcAmount.Text, out req) || req <= 0m)
        { args.IsValid = false; return; }

        var auth = Session["auth"] as UserSession;
        if (auth == null) { args.IsValid = false; return; }

        var rem = _bll.GetCreditNoteRemainingByUser(txtNcId.Text, auth.UserId);
        args.IsValid = (req <= rem && rem > 0m);
    }

    protected void valAccFunds_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!chkAccount.Checked) { args.IsValid = true; return; }
        var auth = Session["auth"] as UserSession; if (auth == null) { args.IsValid = false; return; }
        decimal req; if (!TryParseAmount(txtAccountAmount.Text, out req)) { args.IsValid = false; return; }
        var bal = _bll.GetAccountBalance(auth.UserId);
        args.IsValid = (req <= bal);
    }


   

    protected void btnPagar_Click(object sender, EventArgs e)
    {
        rxCard.Enabled = chkCard.Checked;
        valCardAmt.Enabled = chkCard.Checked;

        rxNc.Enabled = chkNC.Checked;
        valNcIdRequired.Enabled = chkNC.Checked;
        valNcAmt.Enabled = chkNC.Checked;
        valNcFunds.Enabled = chkNC.Checked;

        rxAcc.Enabled = chkAccount.Checked;
        valAccAmt.Enabled = chkAccount.Checked;
        valAccFunds.Enabled = chkAccount.Checked;

        // Validar: tarjeta solo si corresponde; siempre reglas de checkout
        if (chkCard.Checked) Page.Validate("pay");
        Page.Validate("chk");
        if (!Page.IsValid) return;

        // ... armar splits y continuar (igual que ya lo tenés)
        var auth = Session["auth"] as UserSession;
        if (auth == null) { Response.Redirect("/Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl)); return; }

        string planCode = Request.QueryString["plan"];
        string currency = "USD";

        var splits = new List<BEPaymentSplit>();

        if (chkCard.Checked)
        {
            decimal cardAmount; TryParseAmount(txtCardAmount.Text, out cardAmount);
            splits.Add(paymentForm.ToPaymentSplit(cardAmount));
        }
        if (chkNC.Checked)
        {
            decimal a; TryParseAmount(txtNcAmount.Text, out a);
            splits.Add(new BEPaymentSplit { Method = "CREDIT_NOTE", Amount = a, RefId = txtNcId.Text });
        }
        if (chkAccount.Checked)
        {
            decimal a; TryParseAmount(txtAccountAmount.Text, out a);
            splits.Add(new BEPaymentSplit { Method = "ACCOUNT", Amount = a });
        }

        int orderId = _bll.Checkout(auth.UserId, planCode, currency, splits, auth.Email);

        outputmessage.Visible = true;
        outputmessage.Text = "Orden exitosa:  " + orderId.ToString();


    }

}
