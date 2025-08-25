using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web;
using BE;
using BLL;

public partial class Bitacora : System.Web.UI.Page
{
    protected UserSession CurrentUser
    {
        get
        {
            return (Session != null) ? (Session["auth"] as UserSession) : null;
        }
    }



    protected void Page_Load(object sender, EventArgs e)
    {
        ProtegerSoloAdmin();
        if (!IsPostBack)
        {
            Buscar();
        }
    }

    void ProtegerSoloAdmin()
    {
        bool esAdmin = CurrentUser.IsInRole("Admin"); 
        if (!esAdmin) Response.Redirect("/Home.aspx");
    }


    protected void btnFiltrar_Click(object sender, EventArgs e){
        Buscar();
    } 
    protected void btnLimpiar_Click(object sender, EventArgs e)
    {
        txtDesde.Text = txtHasta.Text = txtTexto.Text = "";
        ddlUsuario.ClearSelection(); ddlUsuario.Items[0].Selected = true;
        ddlAgente.ClearSelection(); ddlAgente.Items[0].Selected = true;
        Buscar();
    }

    void Buscar()
    {
        var _bll = new BLLBitacora();
        var filtro = new BEBitacoraFiltro
        {
            UserId = string.IsNullOrEmpty(ddlUsuario.SelectedValue) ? (int?)null : int.Parse(ddlUsuario.SelectedValue),
            Agente = string.IsNullOrEmpty(ddlAgente.SelectedValue) ? null : ddlAgente.SelectedValue,
            Texto = string.IsNullOrWhiteSpace(txtTexto.Text) ? null : txtTexto.Text.Trim(),
            DesdeUtc = ParseLocalDateToUtc(txtDesde.Text),
            HastaUtc = ParseLocalDateToUtc(txtHasta.Text)
        };

        var datos = _bll.Buscar(filtro);
        gvBitacora.DataSource = datos;   // podés bindear Fecha (string) o formatear acá
        gvBitacora.DataBind();
        lblTotal.Text = datos.Count.ToString();
    }

    DateTime? ParseLocalDateToUtc(string yyyyMMdd)
    {
        if (string.IsNullOrWhiteSpace(yyyyMMdd)) return null;
        DateTime d;
        if (DateTime.TryParseExact(yyyyMMdd, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out d))
            return d.ToUniversalTime();
        return null;
    }

   
    protected void gvBitacora_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvBitacora.PageIndex = e.NewPageIndex;
        Buscar();
    }

    protected void gvBitacora_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
        {
            // Columna 0 = FechaUtc (la convierto a hora local elegante)
            var utc = (DateTime)DataBinder.Eval(e.Row.DataItem, "FechaUtc");
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time"); // ajustá tu zona si hace falta
            var local = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
            e.Row.Cells[0].Text = local.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
