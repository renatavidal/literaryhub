using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        var sections = new List<GenreSection>();
        foreach (var g in Genres)
        {
            var books = GetBooksForGenre(g, 10, "es"); // 10 libros, español (cambia a null para todos)
            sections.Add(new GenreSection { Genre = g, Books = books });
        }

        rptSections.DataSource = sections;
        rptSections.DataBind();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        var q = (txtSearch.Text ?? "").Trim();
        if (q.Length == 0) return;
        // Simple: redirigí a una página de búsqueda tuya
        Response.Redirect("~/Search.aspx?q=" + Server.UrlEncode(q));
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
        return s.Length <= max ? s : s.Substring(0, max - 1) + "…";
    }

    // ------------------ VM y DTOs JSON ------------------
    public class GenreSection { public string Genre { get; set; } public List<BookVM> Books { get; set; } }

    public class BookVM
    {
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
