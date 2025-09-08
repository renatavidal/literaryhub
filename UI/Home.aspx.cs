using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using BE;
using BLL;
using System.Net.NetworkInformation;
using System.Web.UI;


public partial class Home : ReaderPage
{
    // Requiere login y email verificado
    protected override bool RequireLogin { get { return true; } }
    protected override bool RequireVerifiedEmail { get { return true; } }
    protected override string[] RequiredRoles { get { return null; } }

    // Generos a mostrar (podés ajustar idioma con langRestrict)
    private static readonly string[] Genres = new[] {
        "Fiction", "Fantasy", "Romance", "Mystery",
        "Science", "History", "Self-Help", "Poetry"
    };

    private static string S(JToken t)
    {
        return t == null ? "" : (string)t;
    }

    private static BookVM MapFromGoogle(JToken item)
    {
        var vi = item == null ? null : item["volumeInfo"];
        var img = vi == null ? null : vi["imageLinks"];

        // Thumbnail con fallback
        string thumb = "/Content/blank-cover.png";
        if (img != null)
        {
            string t1 = S(img["thumbnail"]);
            string t2 = S(img["smallThumbnail"]);
            if (!string.IsNullOrEmpty(t1)) thumb = t1;
            else if (!string.IsNullOrEmpty(t2)) thumb = t2;
        }

        // Autores
        string authors = "";
        var aTok = vi == null ? null : vi["authors"];
        if (aTok != null && aTok.Type == JTokenType.Array)
        {
            var arr = aTok.ToObject<string[]>();
            if (arr != null && arr.Length > 0)
                authors = string.Join(", ", arr);
        }

        // InfoLink
        string infoLink = S(vi == null ? null : vi["infoLink"]);
        if (string.IsNullOrEmpty(infoLink)) infoLink = S(item["selfLink"]);

        return new BookVM
        {
            Gid = S(item["id"]),
            Title = S(vi == null ? null : vi["title"]),
            Authors = authors,
            Thumbnail = thumb,
            InfoLink = infoLink,
            PriceLabel = "" // si no tenés precio, dejalo vacío o poné "-"
        };
    }
    protected string BookUrl(object gidObj, string action = null)
    {
        var gid = gidObj == null ? "" : gidObj.ToString();
        var baseUrl = ResolveUrl("~/BookDetails.aspx");
        if (string.IsNullOrEmpty(gid)) return baseUrl; // fallback

        var qs = "gid=" + Server.UrlEncode(gid);
        if (!string.IsNullOrEmpty(action))
            qs += "&action=" + Server.UrlEncode(action);

        return baseUrl + "?" + qs;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var ph = (string)GetGlobalResourceObject("Global", "Search_Placeholder");
            var q = Request["q"] ?? "";
            BookSearch1.Query = q;
            if (!string.IsNullOrWhiteSpace(q)) BookSearch1.Search();

            var sections = new List<GenreSection>();
            foreach (var g in Genres)
            {
                var books = GetBooksForGenre(g, 10, "es"); // 10 libros, español (cambia a null para todos)
                sections.Add(new GenreSection { Genre = g, Books = books });
            }

            rptSections.DataSource = sections;
            rptSections.DataBind();
        }
    }
    protected void rptSections_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            return;

        var section = (GenreSection)e.Item.DataItem;         
        var inner = (Repeater)e.Item.FindControl("rptBooks");
        inner.DataSource = section.Books;                    
        inner.DataBind();
    }

 

    // ------------------ Google Books ------------------

    private List<BookVM> GetBooksForGenre(string genre, int max, string lang)
    {
        // Cache por 15 min
        string cacheKey = "gb:" + genre + ":" + (lang ?? "all") + ":" + max;
        var cached = Cache[cacheKey] as List<BookVM>;
        if (cached != null) return cached;

        string key = System.Configuration.ConfigurationManager.AppSettings["GoogleBooksApiKey"]; // opcional
        string url = "https://www.googleapis.com/books/v1/volumes?q=subject:" + Uri.EscapeDataString(genre)
                   + "&maxResults=" + max
                   + "&printType=books&orderBy=relevance";
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
                                     : "-";

                    string price = "";
                    if (sale.listPrice != null && sale.listPrice.amount > 0)
                        price = string.Format("${0:0.00} {1}", sale.listPrice.amount, sale.listPrice.currencyCode);
                    else if (sale.saleability == "FREE")
                        price = "Gratis";
                    else
                        price = "-";

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

            Cache.Insert(cacheKey, list, null, DateTime.UtcNow.AddMinutes(15), System.Web.Caching.Cache.NoSlidingExpiration);
            return list;
        }
        catch
        {
            return new List<BookVM>();
        }
    }
    // Trae lo mínimo del libro por GID (C#5-safe)
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
            {
                if (S(it["type"]) == "ISBN_13") { isbn13 = S(it["identifier"]); break; }
            }
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

    protected void BookAction_Command(object sender, CommandEventArgs e)
    {
        try
        {
            var auth = Session["auth"] as UserSession;
            if (auth.IsInRole("Client"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "err", "alert('Como cliente no puedes guardar libros, create una cuenta como usuario para disfrutar de la experiencia');", true);
                return;
            }
            var gid = (e.CommandArgument ?? "").ToString();
            if (string.IsNullOrEmpty(gid)) return;

            var book = FetchBookByGid(gid);
            var bll = new BLLCatalog();
            var status = (e.CommandName == "read") ? UserBookStatus.Read : UserBookStatus.WantToRead;

            bll.SetUserBookStatus(CurrentUserId, book, status);

            var msg = (status == UserBookStatus.Read) ? "¡Marcado como Leído!" : "¡Marcado como Quiero leer!";
            ScriptManager.RegisterStartupScript(this, GetType(), "ok", "alert('" + msg + "');", true);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "alert('No se pudo guardar: " + Server.HtmlEncode(ex.Message).Replace("'", "\\'") + "');", true);
        }
    }


    private static string HttpGet(string url, int timeoutMs)
    {
        var req = (HttpWebRequest)WebRequest.Create(url);
        req.Method = "GET";
        req.Timeout = timeoutMs;
        req.UserAgent = "LiteraryHub/1.0";
        using (var resp = (HttpWebResponse)req.GetResponse())
        using (var sr = new StreamReader(resp.GetResponseStream()))
        {
            return sr.ReadToEnd();
        }
    }

    private static string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Length <= max ? s : s.Substring(0, max - 1) + ".";
    }

    // ------------------ VM y DTOs JSON ------------------
    public class GenreSection { public string Genre { get; set; } public List<BookVM> Books { get; set; } }

    public class BookVM
    {
        public string Gid { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Thumbnail { get; set; }
        public string InfoLink { get; set; }
        public string PriceLabel { get; set; }
    }

    // JSON (solo campos usados)
    public class GoogleBooksRoot { public Item[] items { get; set; } }
    public class Item
    {
        public string id { get; set; }
        public string selfLink { get; set; }
        public VolumeInfo volumeInfo { get; set; }
        public SaleInfo saleInfo { get; set; }
    }
    public class VolumeInfo
    {
        public string title { get; set; }
        public string[] authors { get; set; }
        public ImageLinks imageLinks { get; set; }
        public string infoLink { get; set; }
    }
    public class ImageLinks { public string smallThumbnail { get; set; } public string thumbnail { get; set; } }
    public class SaleInfo
    {
        public string saleability { get; set; }
        public Price listPrice { get; set; }
    }
    public class Price { public decimal amount { get; set; } public string currencyCode { get; set; } }
}

