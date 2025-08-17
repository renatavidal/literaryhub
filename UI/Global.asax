<%@ Application Language="C#" %>
<script runat="server">
  void Application_Start(object sender, EventArgs e)
  {
    System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
  }
   void Application_AcquireRequestState(object sender, EventArgs e)
  {
    try
    {
      if (Session["auth"] != null) return;

      // Si no hay cookies, no podemos autenticar contra el back
      var cookieHeader = Request.Headers["Cookie"];
      if (string.IsNullOrEmpty(cookieHeader)) return;

      var baseUri = new Uri(Request.Url.GetLeftPart(UriPartial.Authority));
      var apiUri  = new Uri(baseUri, VirtualPathUtility.ToAbsolute("/api/User"));

      using (var handler = new HttpClientHandler { UseCookies = false })
      using (var http = new HttpClient(handler))
      {
        var req = new HttpRequestMessage(HttpMethod.Get, apiUri);
        // reenviar las mismas cookies del usuario a la API
        req.Headers.TryAddWithoutValidation("Cookie", cookieHeader);

        var resp = http.SendAsync(req).Result;
        if (!resp.IsSuccessStatusCode) return;

        var json = resp.Content.ReadAsStringAsync().Result;
        var me = JsonConvert.DeserializeObject<MeResponse>(json);

        if (me != null && me.authenticated)
        {
          Session["auth"] = new UserSession {
            UserId = me.userId,
            Email = me.email,
            EmailVerified = me.emailVerified,
            Roles = me.roles ?? new string[0]
          };
        }
      }
    }
    catch { /* silencioso: si falla, el check de tus base pages redirigirá a Login */ }
  }
</script>
