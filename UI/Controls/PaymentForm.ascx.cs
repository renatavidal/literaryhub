using System;
using System.Text.RegularExpressions;
using BE;
using BLL;

public partial class Controls_PaymentForm : System.Web.UI.UserControl
{
    // Exponemos los ClientID de los hidden para que Purchase.aspx los pueda setear por JS
    public string HidGidClientID { get { return hidGid.ClientID; } }
    public string HidTitleClientID { get { return hidTitle.ClientID; } }
    public string HidAuthorsClientID { get { return hidAuthors.ClientID; } }
    public string HidThumbClientID { get { return hidThumb.ClientID; } }
    public string HidIsbn13ClientID { get { return hidIsbn13.ClientID; } }
    public string HidPubClientID { get { return hidPub.ClientID; } }
    public string HidPriceClientID { get { return hidPrice.ClientID; } }
    public string HidCurrencyClientID { get { return hidCurrency.ClientID; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Meses 01..12
            ddlMonth.Items.Clear();
            for (int m = 1; m <= 12; m++)
            {
                string t = (m < 10 ? "0" + m : m.ToString());
                ddlMonth.Items.Add(new System.Web.UI.WebControls.ListItem(t, m.ToString()));
            }
            // Años: actual..+12
            int y = DateTime.UtcNow.Year;
            ddlYear.Items.Clear();
            for (int i = 0; i <= 12; i++)
                ddlYear.Items.Add(new System.Web.UI.WebControls.ListItem((y + i).ToString(), (y + i).ToString()));

            var ph = GetLocalResourceObject("Pay_Number_Placeholder") as string;
            if (!string.IsNullOrEmpty(ph)) txtNumber.Attributes["placeholder"] = ph;
        }
    }

   
    protected void btnPay_Click(object sender, EventArgs e)
    {
        Page.Validate("pay");
        if (!Page.IsValid) return;
        try
        {
            // 1) Book desde hidden
            var book = new BEBook
            {
                GoogleVolumeId = hidGid.Value,
                Title = hidTitle.Value,
                Authors = hidAuthors.Value,
                ThumbnailUrl = hidThumb.Value,
                Isbn13 = hidIsbn13.Value,
                PublishedDate = hidPub.Value
            };

            // 2) Encriptar PAN
            string pan = OnlyDigits(txtNumber.Text);
            byte[] cipher, iv;
            CryptoUtil.EncryptPan(pan, out cipher, out iv);

            // 3) Método de pago (NO guardamos CVV)
            var pm = new BEPaymentMethod
            {
                CardholderName = (txtName.Text ?? "").Trim(),
                CardBrand = DetectBrand(pan),
                Last4 = pan.Length >= 4 ? pan.Substring(pan.Length - 4) : pan,
                ExpMonth = Convert.ToByte(ddlMonth.SelectedValue),
                ExpYear = Convert.ToInt16(ddlYear.SelectedValue),
                PanEncrypted = cipher,
                Iv = iv
            };

            // 4) Precio/Currency (fallback)
            decimal price = 0m;
            decimal.TryParse(hidPrice.Value, out price);
            string curr = string.IsNullOrEmpty(hidCurrency.Value) ? "USD" : hidCurrency.Value;

            // 5) User actual
            var auth = this.Page.Session["auth"] as UserSession;
            if (auth == null) { this.Page.Response.Redirect("/Login.aspx?returnUrl=" + this.Page.Server.UrlEncode(this.Page.Request.RawUrl)); return; }

            // 6) Crear compra (queda Pending)
            var bll = new BLLCatalog();
            int purchaseId = bll.SavePaymentMethodAndPurchase(auth.UserId, book, pm, price, curr);

            // 7) Redirigir a confirmación
            this.Page.Response.Redirect("~/PurchaseConfirmation.aspx?id=" + purchaseId);
        }
        catch (Exception ex)
        {
            var prefix = (GetLocalResourceObject("Pay_ErrorPrefix") as string) ?? "No se pudo procesar el pago: ";
            litStatus.Text = "<div class='hint' style='color:#b00'>" + prefix + this.Page.Server.HtmlEncode(ex.Message) + "</div>";
        }
    }
    protected void valLuhn_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        string n = OnlyDigits(txtNumber.Text);
        args.IsValid = (n.Length >= 12 && n.Length <= 19 );
    }

    protected void valExp_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        int m, y;
        if (!int.TryParse(ddlMonth.SelectedValue, out m) ||
            !int.TryParse(ddlYear.SelectedValue, out y)) { args.IsValid = false; return; }

        var last = DateTime.DaysInMonth(y, m);
        var exp = new DateTime(y, m, last, 23, 59, 59, DateTimeKind.Utc);
        args.IsValid = (exp >= DateTime.UtcNow);
    }

    protected void valCvv_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        string pan = OnlyDigits(txtNumber.Text);
        string brand = DetectBrand(pan);
        string cvv = OnlyDigits(txtCvv.Text);

        if (brand == "AMEX")
            args.IsValid = (cvv.Length == 4);
        else
            args.IsValid = (cvv.Length == 3);
    }

    // -------- helpers --------

    private static string OnlyDigits(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        var arr = new System.Text.StringBuilder();
        for (int i = 0; i < s.Length; i++)
            if (char.IsDigit(s[i])) arr.Append(s[i]);
        return arr.ToString();
    }



    private static string DetectBrand(string pan)
    {
        if (Regex.IsMatch(pan, @"^4\d{12}(\d{3})?(\d{3})?$")) return "VISA";
        if (Regex.IsMatch(pan, @"^(5[1-5]\d{4}|222[1-9]\d{2}|22[3-9]\d{3}|2[3-6]\d{4}|27[01]\d{3}|2720\d{2})\d{10}$")) return "MASTERCARD";
        if (Regex.IsMatch(pan, @"^3[47]\d{13}$")) return "AMEX";
        return "CARD";
    }
}
