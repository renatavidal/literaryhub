using System;
using System.Linq;
using System.Web.Services;
using System.Web.Script.Services;
using BLL;
using BE;

public partial class SupportChat : ReaderPage
{
    protected override bool RequireLogin { get { return true; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var auth = Session["auth"] as UserSession;
            int uid = (auth != null) ? auth.UserId : 0;
            int tid = new BLLChat().GetOrCreateOpenThread(uid);
            hfThreadId.Value = tid.ToString();
        }
    }

    private static int CurrentUserId()
    {
        var auth = (System.Web.HttpContext.Current.Session["auth"] as UserSession);
        return (auth != null) ? auth.UserId : 0;
    }
    private static bool IsAdmin()
    {
        var auth = (System.Web.HttpContext.Current.Session["auth"] as UserSession);
        var roles = (auth != null && auth.Roles != null) ? auth.Roles : new string[0];
        return Array.Exists(roles, r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase));
    }

    public class ApiItems { public object[] items { get; set; } }
    public class ApiOk { public bool ok { get; set; } public string error { get; set; } }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiItems GetSince(int threadId, long sinceId)
    {
        int uid = CurrentUserId();
        // seguridad básica: el cliente solo puede leer su hilo
        var bll = new BLLChat();
        // (si querés, validá que el thread pertenezca a uid)
        var items = bll.GetMessagesSince(threadId, sinceId)
            .Select(m => new { id = m.Id, isAdmin = m.IsAdmin, body = m.Body, createdUtc = m.CreatedUtc.ToString("o") })
            .Cast<object>().ToArray();
        return new ApiItems { items = items };
    }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiOk Send(int threadId, string text)
    {
        try
        {
            int uid = CurrentUserId();
            var bll = new BLLChat();
            bll.SendMessage(threadId, uid, false, text);
            return new ApiOk { ok = true };
        }
        catch (Exception ex) { return new ApiOk { ok = false, error = ex.Message }; }
    }
}
