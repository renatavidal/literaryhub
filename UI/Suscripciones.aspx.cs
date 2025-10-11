using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BLL;

public partial class Suscripciones : ReaderPage
{
    private const string VS_PLANES = "planes";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var planes = CargarPlanesDesdeBLL();      // trae y normaliza
            ViewState[VS_PLANES] = planes;

            BindListado(planes);
            BindSelectors(planes);
            PreselectFromQuery();
            BindSubscription();
            rptPlanes.Visible = true;
        }
    }
    protected void btnCancelSub_Click(object sender, EventArgs e)
    {
        var auth = Session["auth"] as UserSession;
        if (auth == null) return;

        try
        {
            new BLL.BLLSubscription().CancelForUser(auth.UserId);

            // Actualizar la sesión para reflejar que quedó como Reader
            auth.Roles = new[] { "Reader" };
            Session["auth"] = auth;

            lblSubMsg.Text = "Suscripción cancelada. Tu rol ahora es Reader.";
        }
        catch (Exception ex)
        {
            lblSubMsg.Text = "Error: " + Server.HtmlEncode(ex.Message);
        }
    }

    private void BindSubscription()
    {
        var auth = Session["auth"] as UserSession;
        if (auth == null) return;

        var bll = new BLLSubscription();
        var sub = bll.GetActiveForUser(auth.UserId);
        if (sub == null || !sub.IsActive)
        {
            return;
        }

        litSubName.Text = Server.HtmlEncode(sub.PlanName ?? ("Plan #" + sub.PlanId));

        string dates = "";
        if (sub.PaidUtc.HasValue) dates += "Desde: " + sub.PaidUtc.Value.ToString("yyyy-MM-dd HH:mm") + " UTC";
        if (sub.ExpiresUtc.HasValue) dates += " — Hasta: " + sub.ExpiresUtc.Value.ToString("yyyy-MM-dd HH:mm") + " UTC";
        litSubDates.Text = dates;

        // Botón Valorar → CheckoutOk con id del plan
        lnkValorarPlan.NavigateUrl = ResolveUrl("~/CheckoutOk.aspx?id=" + sub.PlanId);
    }

    // -------- CARGA ROBUSTA DESDE BLL --------
    private List<PlanDto> CargarPlanesDesdeBLL()
    {
        var bll = new BLLUsuario();
        object raw = bll.ListarPlanesSuscripcion();

        var lista = new List<PlanDto>();

        // Caso A: DataTable
        var dt = raw as DataTable;
        if (dt != null)
        {
            foreach (DataRow r in dt.Rows)
                lista.Add(MapRow(r));
            return FiltrarOrdenar(lista);
        }

        // Caso B: DataView
        var dv = raw as DataView;
        if (dv != null)
        {
            foreach (DataRowView rv in dv)
                lista.Add(MapRow(rv.Row));
            return FiltrarOrdenar(lista);
        }

        // Caso C: IEnumerable de objetos (List<>, anonymous, etc.)
        var enumerable = raw as IEnumerable;
        if (enumerable != null)
        {
            foreach (var obj in enumerable)
                lista.Add(MapObject(obj));
            return FiltrarOrdenar(lista);
        }

        // Caso desconocido: intento mapear directo
        if (raw != null) lista.Add(MapObject(raw));
        return FiltrarOrdenar(lista);
    }

    private static List<PlanDto> FiltrarOrdenar(List<PlanDto> src)
    {
        bool hayAlMenosUnoActivo = false;
        foreach (var x in src) { if (x.Activo) { hayAlMenosUnoActivo = true; break; } }

        var list = hayAlMenosUnoActivo ? src.FindAll(x => x.Activo) : src;
        list.Sort((a, b) => a.PrecioUSD.CompareTo(b.PrecioUSD));
        return list;
    }


    private static PlanDto MapRow(DataRow r)
    {
        return new PlanDto
        {
            Codigo = GetCell<string>(r, "Codigo"),
            Descripcion = GetCell<string>(r, "Descripcion"),
            Roles = GetCell<string>(r, "Roles"),
            PrecioUSD = ToDecimal(GetCell<object>(r, "PrecioUSD")),
            EsDestacado = ToBool(GetCell<object>(r, "EsDestacado")),
            Activo = ToBool(GetCell<object>(r, "Activo"))
        };
    }

    private static T GetCell<T>(DataRow r, string col)
    {
        if (!r.Table.Columns.Contains(col) || r[col] == DBNull.Value) return default(T);
        object val = r[col];
        try { return (T)Convert.ChangeType(val, typeof(T)); } catch { return default(T); }
    }

    private static PlanDto MapObject(object obj)
    {
        return new PlanDto
        {
            Codigo = GetProp<string>(obj, "Codigo"),
            Descripcion = GetProp<string>(obj, "Descripcion"),
            Roles = GetProp<string>(obj, "Roles"),
            PrecioUSD = ToDecimal(GetProp<object>(obj, "PrecioUSD")),
            EsDestacado = ToBool(GetProp<object>(obj, "EsDestacado")),
            Activo = ToBool(GetProp<object>(obj, "Activo"))
        };
    }

    private static T GetProp<T>(object obj, string name)
    {
        if (obj == null) return default(T);
        var p = obj.GetType().GetProperty(name);
        if (p == null) return default(T);
        var v = p.GetValue(obj, null);
        if (v == null) return default(T);
        try { return (T)Convert.ChangeType(v, typeof(T)); } catch { return default(T); }
    }

    private static decimal ToDecimal(object v)
    {
        if (v == null) return 0m;
        try { return Convert.ToDecimal(v); } catch { return 0m; }
    }
    private static bool ToBool(object v)
    {
        if (v == null) return false;
        try { return Convert.ToInt32(v) != 0; } catch { }
        try { return Convert.ToBoolean(v); } catch { return false; }
    }

    // -------- BINDEOS --------
    private void BindListado(List<PlanDto> planes)
    {
        rptPlanes.ItemCommand += rptPlanes_ItemCommand;
        rptPlanes.DataSource = planes;
        rptPlanes.DataBind();
    }

    private void BindSelectors(List<PlanDto> planes)
    {
        FillSelector(ddlA, planes);
        FillSelector(ddlB, planes);
        FillSelector(ddlC, planes);
    }

    private static void FillSelector(DropDownList ddl, List<PlanDto> planes)
    {
        ddl.Items.Clear();
        ddl.Items.Add(new ListItem("— seleccionar —", ""));
        foreach (var p in planes)
        {
            var txt = string.Format("{0} (${1:0.##})", p.Descripcion, p.PrecioUSD);
            ddl.Items.Add(new ListItem(txt, p.Codigo));
        }
    }

    // -------- COMPARACIÓN --------
    protected void btnCompare_Click(object sender, EventArgs e)
    {
        rptPlanes.Visible = false;
        var codes = new[] { ddlA.SelectedValue, ddlB.SelectedValue, ddlC.SelectedValue }
            .Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();


        if (codes.Count < 2)
        {
            phCompare.Controls.Clear();
            phCompare.Controls.Add(new Literal { Text = "<div class='muted'>Elegí al menos dos planes para comparar.</div>" });
            rptPlanes.Visible = true;
            return;
        }

        var planes = (List<PlanDto>)ViewState[VS_PLANES];
        var pick = planes.Where(p => codes.Contains(p.Codigo)).ToList();

        var html = new StringBuilder();
        html.Append("<div class='compare-grid'><div class='compare-cols'>");

        foreach (var p in pick)
        {
            var colClass = p.EsDestacado ? "compare-col popular" : "compare-col";
            html.AppendFormat("<div class='{0}'>", colClass);
            html.Append("<div class='compare-head'>");
            html.AppendFormat("<span class='badge mini'>{0}</span>", System.Web.HttpUtility.HtmlEncode(p.Codigo));
            html.AppendFormat("<div style='font-weight:700'>{0}</div>", System.Web.HttpUtility.HtmlEncode(p.Descripcion));
            html.AppendFormat("<div class='compare-price'>$ {0:0.##} <span class='per'>/mes</span></div>", p.PrecioUSD);
            html.Append("</div>");

            html.Append("<ul class='compare-features'>");
            html.AppendFormat("<li><strong>Roles</strong>: {0}</li>", System.Web.HttpUtility.HtmlEncode(p.Roles));
            html.AppendFormat("<li><strong>Destacado</strong>: {0}</li>", p.EsDestacado ? "Sí" : "No");
            html.AppendFormat("<li><strong>Activo</strong>: {0}</li>", p.Activo ? "Sí" : "No");
            html.Append("<li>Acceso a comunidad ✓</li>");
            html.Append("<li>Panel de usuario ✓</li>");
            html.Append("<li>Soporte básico ✓</li>");
            html.Append("</ul>");

            html.AppendFormat("<div style='margin-top:12px'><a class='btn-choose' href='/Checkout.aspx?plan={0}'>Elegir</a></div>",
                System.Web.HttpUtility.UrlEncode(p.Codigo));
            html.Append("</div>");
        }

        html.Append("</div></div>");
        phCompare.Controls.Clear();
        phCompare.Controls.Add(new Literal { Text = html.ToString() });
    }

    private void rptPlanes_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "prefill")
        {
            var code = Convert.ToString(e.CommandArgument);
            var slots = new[] { ddlA, ddlB, ddlC };
            var already = slots.Any(d => d.SelectedValue == code);
            if (!already)
            {
                DropDownList firstEmpty = null;
                foreach (var d in slots)
                    if (string.IsNullOrEmpty(d.SelectedValue)) { firstEmpty = d; break; }
                (firstEmpty ?? ddlC).SelectedValue = code;
            }
        }
    }

    private void PreselectFromQuery()
    {
        var q = Request.QueryString["compare"];
        if (string.IsNullOrEmpty(q)) return;
        var codes = q.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Distinct().ToList();
        var slots = new[] { ddlA, ddlB, ddlC };
        for (int i = 0; i < slots.Length && i < codes.Count; i++)
            if (slots[i].Items.FindByValue(codes[i]) != null)
                slots[i].SelectedValue = codes[i];
    }

    [Serializable]
    private class PlanDto
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Roles { get; set; }
        public decimal PrecioUSD { get; set; }
        public bool EsDestacado { get; set; }
        public bool Activo { get; set; }
    }
}
