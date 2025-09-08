using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using BE;
using BLL;

public partial class Controls_BookSearch : UserControl
{
    // Propiedad para pre-cargar query (?q=)
    public string Query
    {
        get { return txtQ.Text; }
        set { txtQ.Text = value ?? ""; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var ph = (string)HttpContext.GetGlobalResourceObject("Global", "Search_Placeholder");
            if (!string.IsNullOrEmpty(ph)) txtQ.Attributes["placeholder"] = ph;
        }
    }

    // Expuesto para que Search.aspx pueda disparar búsqueda inicial
    public void Search()
    {
        DoSearch(txtQ.Text);
    }

    protected void btnGo_Click(object sender, EventArgs e)
    {
        DoSearch(txtQ.Text);
    }

    private void DoSearch(string q)
    {
        q = (q ?? "").Trim();
        if (q.Length == 0)
        {
            pnlResults.Visible = false;
            rpt.DataSource = null;
            rpt.DataBind();
            return;
        }

        string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName; // "es", "en"...
        var list = GetBooksByTitle(q, 24, lang);
        rpt.DataSource = list;
        rpt.DataBind();
        pnlResults.Visible = true;
    }

    // ====== Acciones (igual que en Home) ======
    protected void BookAction_Command(object sender, CommandEventArgs e)
    {
        try
        {
            var gid = (e.CommandArgument ?? "").ToString();
            if (string.IsNullOrEmpty(gid)) return;

            var book = FetchBookByGid(gid);
            var bll = new BLLCatalog();
            var status = (e.CommandName == "read") ? UserBookStatus.Read : UserBookStatus.WantToRead;

            bll.SetUserBookStatus(CurrentUserId, book, status);

            var msg = (status == UserBookStatus.Read) ? "¡Marcado como Leído!" : "¡Marcado como Quiero leer!";
            ScriptManager.RegisterStartupScript(Page, GetType(), "ok", "alert('" + msg + "');", true);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "err",
                "alert('No se pudo guardar: " + Page.Server.HtmlEncode(ex.Message).Replace("'", "\\'") + "');", true);
        }
    }

    private int CurrentUserId
    {
        get
        {
            var auth = (Page.Session != null) ? Page.Session["auth"] as UserSession : null;
            if (auth == null)
            {
                var ret = Page.Server.UrlEncode(Page.Request.RawUrl);
                Page.Response.Redirect("/Login.aspx?returnUrl=" + ret);
                return 0;
            }
            return auth.UserId;
        }
    }

    // ====== Google Books ======
    private List<BookVM> GetBooksByTitle(string title, int max, string lang)
    {
        string key = System.Configuration.ConfigurationManager.AppSettings["GoogleBooksApiKey"];
        string url = "https://www.googleapis.com/books/v1/volumes?q=intitle:" + Uri.EscapeDataString(title)
                   + "&maxResults=" + max + "&printType=books&orderBy=relevance";
        if (!string.IsNullOrEmpty(lang)) url += "&langRestrict=" + lang;
        if (!string.IsNullOrEmpty(key)) url += "&key=" + key;

        try
        {
            string json = HttpGet(url, 8000);
            var ser = new JavaScriptSerializer();
            var root = ser.Deserialize<GoogleBooksRoot>(json);

            var list = new List<BookVM>();
            if (root != null && root.items != null)
            {
                foreach (var it in root.items)
                {
                    var vi = it.volumeInfo ?? new VolumeInfo();
                    var sale = it.saleInfo ?? new SaleInfo();

                    string thumb = (vi.imageLinks != null ? (vi.imageLinks.thumbnail ?? vi.imageLinks.smallThumbnail) : null)
                                   ?? "https://via.placeholder.com/300x420?text=No+Cover";

                    string authors = (vi.authors != null && vi.authors.Length > 0)
                                     ? string.Join(", ", vi.authors)
                                     : "—";

                    string price = "";
                    if (sale.listPrice != null && sale.listPrice.amount > 0)
                        price = string.Format("${0:0.00} {1}", sale.listPrice.amount, sale.listPrice.currencyCode);
                    else if (sale.saleability == "FREE")
                        price = "Gratis";
                    else
                        price = "—";

                    list.Add(new BookVM
                    {
                        Gid = it.id ?? "",
                        Title = Truncate(vi.title, 80),
                        Authors = Truncate(authors, 80),
                        Thumbnail = thumb.Replace("http://", "https://"),
                        InfoLink = vi.infoLink ?? (it.selfLink ?? "#"),
                        PriceLabel = price
                    });
                }
            }
            return list;
        }
        catch { return new List<BookVM>(); }
    }

    private static BEBook FetchBookByGid(string gid)
    {
        if (string.IsNullOrWhiteSpace(gid)) return null;

        string url = "https://www.googleapis.com/books/v1/volumes/" + Uri.EscapeDataString(gid);
        string json = HttpGet(url, 7000);
        var jo = JObject.Parse(json);
        var vi = jo["volumeInfo"];

        string authors = "";
        var aTok = vi == null ? null : vi["authors"];
        if (aTok != null && aTok.Type == JTokenType.Array)
        {
            var arr = aTok.ToObject<string[]>();
            if (arr != null && arr.Length > 0) authors = string.Join(", ", arr);
        }

        string img = "";
        var il = vi == null ? null : vi["imageLinks"];
        if (il != null) img = S(il["thumbnail"] ?? il["smallThumbnail"]);

        string isbn13 = "";
        var ids = vi == null ? null : vi["industryIdentifiers"];
        if (ids != null && ids.Type == JTokenType.Array)
        {
            foreach (var it in ids)
                if (S(it["type"]) == "ISBN_13") { isbn13 = S(it["identifier"]); break; }
        }

        return new BEBook
        {
            GoogleVolumeId = gid,
            Title = S(vi == null ? null : vi["title"]),
            Authors = authors,
            ThumbnailUrl = img,
            Isbn13 = isbn13,
            PublishedDate = S(vi == null ? null : vi["publishedDate"])
        };
    }

    // Helpers y DTOs
    private static string HttpGet(string url, int timeoutMs)
    {
        var req = (HttpWebRequest)WebRequest.Create(url);
        req.Method = "GET"; req.Timeout = timeoutMs; req.UserAgent = "LiteraryHub/1.0";
        using (var resp = (HttpWebResponse)req.GetResponse())
        using (var sr = new StreamReader(resp.GetResponseStream())) return sr.ReadToEnd();
    }
    private static string S(JToken t) { return t == null ? "" : (string)t; }
    private static string Truncate(string s, int max) { if (string.IsNullOrEmpty(s)) return ""; return s.Length <= max ? s : s.Substring(0, max - 1) + "…"; }

    public string BookUrl(object gidObj, string action = null)
    {
        var gid = gidObj == null ? "" : gidObj.ToString();
        var baseUrl = ResolveClientUrl("~/BookDetails.aspx");
        if (string.IsNullOrEmpty(gid)) return baseUrl;
        var qs = "gid=" + Page.Server.UrlEncode(gid);
        if (!string.IsNullOrEmpty(action)) qs += "&action=" + Page.Server.UrlEncode(action);
        return baseUrl + "?" + qs;
    }

    // ViewModel + JSON DTOs (solo campos usados)
    public class BookVM
    {
        public string Gid { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Thumbnail { get; set; }
        public string InfoLink { get; set; }
        public string PriceLabel { get; set; }
    }
    public class GoogleBooksRoot { public Item[] items { get; set; } }
    public class Item { public string id { get; set; } public string selfLink { get; set; } public VolumeInfo volumeInfo { get; set; } public SaleInfo saleInfo { get; set; } }
    public class VolumeInfo { public string title { get; set; } public string[] authors { get; set; } public ImageLinks imageLinks { get; set; } public string infoLink { get; set; } }
    public class ImageLinks { public string smallThumbnail { get; set; } public string thumbnail { get; set; } }
    public class SaleInfo { public string saleability { get; set; } public Price listPrice { get; set; } }
    public class Price { public decimal amount { get; set; } public string currencyCode { get; set; } }
}
