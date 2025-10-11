using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using BLL;

public partial class AdminReports : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsAdmin())
        {
            Response.Redirect("~/AccessDenied.aspx");
            return;
        }
    }

    private bool IsAdmin()
    {
        var auth = (Session["auth"] as UserSession) ?? (Session["auth1"] as UserSession);
        if (auth == null) return false;

        // En tu modelo Roles es string[]; ajustá si fuese CSV.
        var roles = auth.Roles ?? new string[0];
        return Array.Exists(roles, r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase));
    }

    // =======================
    //   GANANCIAS (Revenue)
    // =======================

    /// <summary>
    /// Devuelve series agregadas de revenue según granularidad:
    /// 'YEAR' | 'MONTH' | 'WEEK' | 'DAY'
    /// Fechas en ISO UTC opcionales; currency opcional (ej: "USD").
    /// </summary>
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object Revenue(string granularity, string fromUtc, string toUtc, string currency)
    {
        DateTime? from = ParseIso(fromUtc);
        DateTime? to = ParseIso(toUtc);

        var bll = new BLLRevenue();
        var rows = bll.List((granularity ?? "DAY").ToUpperInvariant(), from, to, currency);

        // Formato liviano para Chart.js
        return new
        {
            items = rows.Select(r => new
            {
                label = r.Label,   // "2025", "2025-09", "2025-09-01", etc.
                amount = r.Amount,  // suma de Price
                count = r.Count    // cantidad de compras
            }).ToList()
        };
    }

    private static DateTime? ParseIso(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;

        DateTime dt;
        if (DateTime.TryParse(
                s,
                null,
                System.Globalization.DateTimeStyles.AdjustToUniversal |
                System.Globalization.DateTimeStyles.AssumeUniversal,
                out dt))
        {
            return dt;
        }
        return null;
    }
}
