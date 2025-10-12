// PagesApi.aspx.cs
using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;

public partial class PagesApi : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e) { }

    private static string[] GetUserRoles()
    {
        var ctx = HttpContext.Current;
        var auth = ctx.Session["auth"] as UserSession; // tu objeto de sesión
        if (auth != null && auth.Roles != null) return auth.Roles;
        return new string[0];
    }

    public class PageDto { public string title; public string url; }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object Suggest(string q)
    {
        var roles = GetUserRoles();
        var items = PageDirectory.Search(q, roles)
                                 .Select(p => new PageDto
                                 {
                                     title = p.Title,
                                     url = VirtualPathUtility.ToAbsolute(p.Url)
                                 })
                                 .ToArray();

        return new { ok = true, items };
    }
}
