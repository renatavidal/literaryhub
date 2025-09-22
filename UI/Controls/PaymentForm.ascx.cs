using System;
using System.Text.RegularExpressions;
using BE;
using BLL;        // para BLLCatalog si lo usás en modo standalone
using Servicios;  // CardUtils y CryptoUtil

public partial class Controls_PaymentForm : System.Web.UI.UserControl
{
    // ====== Modo de operación ======
    private bool _standalone = true;
    public bool Standalone
    {
        get { return _standalone; }
        set { _standalone = value; }
    }

    // ====== Propiedades públicas (para uso desde Checkout) ======
    public string NumberDigits { get { return CardUtils.OnlyDigits(txtNumber.Text); } }
    public string CardBrand { get { return CardUtils.DetectBrand(NumberDigits); } }
    public string Last4 { get { var n = NumberDigits; return n.Length >= 4 ? n.Substring(n.Length - 4) : n; } }
    public byte ExpMonth { get { byte m; return byte.TryParse(ddlMonth.SelectedValue, out m) ? m : (byte)0; } }
    public short ExpYear { get { short y; return short.TryParse(ddlYear.SelectedValue, out y) ? y : (short)0; } }
    public string CardholderName { get { return (txtName.Text ?? "").Trim(); } }

    // (opcionales)
    public string PriceRaw { get { return hidPrice.Value; } }
    public string Currency { get { return string.IsNullOrEmpty(hidCurrency.Value) ? "USD" : hidCurrency.Value; } }

    // ===== Exponer ClientID de los hidden (usado por Purchase.aspx) =====
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
            ddlMonth.Items.Clear();
            for (int m = 1; m <= 12; m++)
            {
                string t = (m < 10 ? "0" + m : m.ToString());
                ddlMonth.Items.Add(new System.Web.UI.WebControls.ListItem(t, m.ToString()));
            }

            int y = DateTime.UtcNow.Year;
            ddlYear.Items.Clear();
            for (int i = 0; i <= 12; i++)
            {
                string ys = (y + i).ToString();
                ddlYear.Items.Add(new System.Web.UI.WebControls.ListItem(ys, ys));
            }

            var ph = GetLocalResourceObject("Pay_Number_Placeholder") as string;
            if (!string.IsNullOrEmpty(ph)) txtNumber.Attributes["placeholder"] = ph;
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        // En modo “colector” el botón interno no debe accionar ni validar
        btnPay.Visible = _standalone;
        btnPay.CausesValidation = _standalone;
    }

    // ===== Validadores =====
    protected void valLuhn_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        string n = NumberDigits;
        args.IsValid = (n.Length >= 12 && n.Length <= 19);
    }

    protected void valExp_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        int m, y;
        if (!int.TryParse(ddlMonth.SelectedValue, out m) || !int.TryParse(ddlYear.SelectedValue, out y))
        { args.IsValid = false; return; }

        int last = DateTime.DaysInMonth(y, m);
        DateTime exp = new DateTime(y, m, last, 23, 59, 59, DateTimeKind.Utc);
        args.IsValid = (exp >= DateTime.UtcNow);
    }

    protected void valCvv_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        string brand = CardBrand;
        string cvv = CardUtils.OnlyDigits(txtCvv.Text);
        args.IsValid = (brand == "AMEX") ? (cvv.Length == 4) : (cvv.Length == 3);
    }

    // ===== Modo “colector”: arma el split cifrado para que lo use Checkout =====
    public BEPaymentSplit ToPaymentSplit(decimal amount)
    {
        byte[] cipher, iv;
        CryptoUtil.EncryptPan(NumberDigits, out cipher, out iv);

        BEPaymentSplit s = new BEPaymentSplit();
        s.Method = "CARD";
        s.Amount = amount;
        s.CardBrand = this.CardBrand;
        s.Last4 = this.Last4;
        s.PanEncrypted = cipher;
        s.PanIV = iv;
        return s;
    }

    // ===== Modo “standalone”: sigue funcionando como antes =====
    protected void btnPay_Click(object sender, EventArgs e)
    {
        Page.Validate("pay");
        if (!Page.IsValid) return;

        // Si está en modo colector, no procesa la compra: lo maneja la página contenedora
        if (!_standalone) return;

        try
        {
            // 1) Book desde hidden (como lo tenías)
            var book = new BEBook
            {
                GoogleVolumeId = hidGid.Value,
                Title = hidTitle.Value,
                Authors = hidAuthors.Value,
                ThumbnailUrl = hidThumb.Value,
                Isbn13 = hidIsbn13.Value,
                PublishedDate = hidPub.Value
            };

            // 2) Cifrar PAN
            byte[] cipher, iv;
            CryptoUtil.EncryptPan(NumberDigits, out cipher, out iv);

            // 3) Método de pago
            var pm = new BEPaymentMethod
            {
                CardholderName = this.CardholderName,
                CardBrand = this.CardBrand,
                Last4 = this.Last4,
                ExpMonth = this.ExpMonth,
                ExpYear = this.ExpYear,
                PanEncrypted = cipher,
                Iv = iv
            };

            // 4) Precio/Currency
            decimal price = 0m; decimal.TryParse(PriceRaw, out price);
            string curr = this.Currency;

            // 5) Usuario
            var auth = this.Page.Session["auth"] as UserSession;
            if (auth == null)
            {
                this.Page.Response.Redirect("/Login.aspx?returnUrl=" +
                    this.Page.Server.UrlEncode(this.Page.Request.RawUrl));
                return;
            }

            // 6) Crear compra (queda Pending)
            var bll = new BLLCatalog();
            int purchaseId = bll.SavePaymentMethodAndPurchase(auth.UserId, book, pm, price, curr);

            // 7) Redirigir a confirmación
            this.Page.Response.Redirect("~/PurchaseConfirmation.aspx?id=" + purchaseId);
        }
        catch (Exception ex)
        {
            var prefix = (GetLocalResourceObject("Pay_ErrorPrefix") as string) ?? "No se pudo procesar el pago: ";
            litStatus.Text = "<div class='hint' style='color:#b00'>" +
                             prefix + this.Page.Server.HtmlEncode(ex.Message) + "</div>";
        }
    }
}
