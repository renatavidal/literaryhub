<%@ Application Language="C#" %>
<script runat="server">
void Application_Start(object sender, System.EventArgs e)
{
    // Tu init...
}

void Application_BeginRequest(object sender, System.EventArgs e)
{
    var req = System.Web.HttpContext.Current.Request;
    var res = System.Web.HttpContext.Current.Response;

    string lang = req.QueryString["lang"];
    if (!string.IsNullOrEmpty(lang))
    {
        var ck = new System.Web.HttpCookie("lh-lang", lang)
        {
            Expires = System.DateTime.UtcNow.AddYears(1),
            HttpOnly = true,
            Path = "/"
        };
        res.Cookies.Add(ck);
    }
    else
    {
        var c = req.Cookies["lh-lang"];
        lang = (c != null && !string.IsNullOrEmpty(c.Value)) ? c.Value : "es";
    }

    try
    {
        var ci = new System.Globalization.CultureInfo(lang);
        System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
        System.Threading.Thread.CurrentThread.CurrentCulture   = ci;
    }
    catch
    {
        var ci = new System.Globalization.CultureInfo("es");
        System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
        System.Threading.Thread.CurrentThread.CurrentCulture   = ci;
    }
}
</script>
