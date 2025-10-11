using System;
using System.Web.Services;
using System.Web.Script.Services;
using BLL;

public partial class AdsPublic : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e) { Response.End(); }

    [WebMethod(EnableSession = false)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetRandom()
    {
        var a = new BLLAdvert().GetRandomActive();
        if (a == null) return new { ad = (object)null };
        return new { ad = new { id = a.Id, title = a.Title, body = a.Body, imageUrl = a.ImageUrl, linkUrl = a.LinkUrl } };
    }
}
