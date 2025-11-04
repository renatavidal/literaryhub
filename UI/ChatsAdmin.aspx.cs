using System;
using System.Linq;
using System.Web.Services;
using System.Web.Script.Services;
using BLL;
using BE;

public partial class ChatsAdmin : Perm_SoporteChatPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    private bool IsAdmin()
    {
        var auth = Session["auth"] as UserSession;
        if (auth == null) return false;
        var roles = auth.Roles ?? new string[0];
        return Array.Exists(roles, r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase));
    }
    private static int CurrentUserId()
    {
        var auth = (System.Web.HttpContext.Current.Session["auth"] as UserSession);
        return (auth != null) ? auth.UserId : 0;
    }

    public class ApiItems { public object[] items { get; set; } }
    public class ApiOk { public bool ok { get; set; } public string error { get; set; } }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiItems List()
    {
        var _bll = new BLLChat();
        var items = _bll.ListThreads()
          .Select(t => new { id = t.Id, customerId = t.CustomerId, status = (int)t.Status, lastMsg = t.LastMsg })
          .Cast<object>().ToArray();
        return new ApiItems { items = items };
    }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiItems GetSince(int threadId, long sinceId)
    {
        var _bll = new BLLChat();
        var items = _bll.GetMessagesSince(threadId, sinceId)
          .Select(m => new { id = m.Id, isAdmin = m.IsAdmin, body = m.Body, createdUtc = m.CreatedUtc.ToString("o") })
          .Cast<object>().ToArray();
        return new ApiItems { items = items };
    }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiOk Send(int threadId, string text)
    {
        try
        {
            if (text.Length >= 201)
            {
                return new ApiOk
                {
                    ok = false,
                    error = "El mensaje no puede superar los 200 caracteres."
                };
            }
            var _bll = new BLLChat();
            _bll.SendMessage(threadId, CurrentUserId(), true, text);
            return new ApiOk { ok = true };
        }
        catch (Exception ex) { return new ApiOk { ok = false, error = ex.Message }; }
    }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiOk Close(int threadId)
    {
        try {
            var _bll = new BLLChat(); 
            _bll.CloseThread(threadId); return new ApiOk { ok = true }; }
        catch (Exception ex) { return new ApiOk { ok = false, error = ex.Message }; }
    }
}
