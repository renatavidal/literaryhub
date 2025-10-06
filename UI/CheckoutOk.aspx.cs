using System;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using BLL;

public partial class CheckoutOk : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int pid = 0;
            int.TryParse(Request.QueryString["pid"], out pid);
            if (pid == 0) int.TryParse(Request.QueryString["id"], out pid);
            if (pid == 0) int.TryParse(Request.QueryString["productId"], out pid);

            hfProductId.Value = pid.ToString();

            int userId = GetCurrentUserId(); // método de instancia para Page_Load/postback
            if (pid > 0 && userId > 0)
            {
                var bll = new BLLProductRating();
                hfInitialRating.Value = bll.GetUserRating(pid, userId).ToString();
            }
        }
    }

    protected void btnSend_Click(object sender, EventArgs e)
    {
        // Fallback sin JS (postback)
        int pid = 0, rating = 0;
        int.TryParse(hfProductId.Value, out pid);
        int.TryParse(Request.Form["rating"], out rating);
        int userId = GetCurrentUserId();
        if (pid > 0 && userId > 0)
        {
            var bll = new BLLProductRating();
            var agg = bll.SaveRating(pid, userId, rating);
            // opcional: actualizar labels/hidden si querés mostrar avg/count en server-side
        }
    }

    // ---- Instancia: lo usa Page_Load / postback
    private int GetCurrentUserId()
    {
        var auth = Session["auth"] as UserSession;
        if (auth != null && auth.UserId > 0) return auth.UserId;

        object o = Session["UserId"];
        int uid; return (o != null && int.TryParse(o.ToString(), out uid)) ? uid : 0;
    }

    // ---- ESTÁTICO: lo usa el WebMethod
    private static int GetUserIdFromSession()
    {
        var ctx = HttpContext.Current;
        if (ctx.Session == null) return 0;

        var auth = ctx.Session["auth"] as UserSession;
        if (auth != null && auth.UserId > 0) return auth.UserId;

        object o = ctx.Session["UserId"];
        int uid; return (o != null && int.TryParse(o.ToString(), out uid)) ? uid : 0;
    }

    public class SaveResult
    {
        public bool ok { get; set; }
        public string error { get; set; }
        public decimal avg { get; set; }
        public int count { get; set; }
    }

    // <<< ESTA es la que llama fetch >>>
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static SaveResult SaveRating(int productId, int rating)
    {
        int userId = GetUserIdFromSession();
        if (userId <= 0)
            return new SaveResult { ok = false, error = "You must be logged in." };

        try
        {
            var bll = new BLLProductRating();
            var agg = bll.SaveRating(productId, userId, rating);
            return new SaveResult { ok = true, avg = agg.Average, count = agg.Count };
        }
        catch (Exception ex)
        {
            return new SaveResult { ok = false, error = ex.Message };
        }
    }
}
