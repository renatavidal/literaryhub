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
            CargarUsuarios();
        }
    }

    void ProtegerSoloAdmin()
    {
        if (CurrentUser != null)
        {
            bool esAdmin = CurrentUser.IsInRole("Admin");
            if (!esAdmin) Response.Redirect("/Home.aspx");
        }
        else
        {
            Response.Redirect("/AccessDenied.aspx");
        }
    }


    protected void btnFiltrar_Click(object sender, EventArgs e){
        Buscar();
    } 
    protected void btnLimpiar_Click(object sender, EventArgs e)
    {
        txtDesde.Text = txtHasta.Text = txtTexto.Text = "";
        ddlUsuario.ClearSelection(); ddlUsuario.Items[0].Selected = true;
        Buscar();
    }
    void CargarUsuarios()
    {
        var bllU = new BLLUsuario();
        var items = bllU.ListarUsuariosParaFiltro(); // Id, Texto

        ddlUsuario.DataSource = items;
        ddlUsuario.DataTextField = "Texto";
        ddlUsuario.DataValueField = "Id";
        ddlUsuario.DataBind();
        ddlUsuario.Items.Insert(0, new System.Web.UI.WebControls.ListItem((GetLocalResourceObject("Audit_Filter_All") as string) ?? "Todos", "")); // primera opción
    }

    // (opcional) mostrar el nombre en la grilla en vez del Id
    protected void gvBitacora_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
        {
            var utc = (DateTime)DataBinder.Eval(e.Row.DataItem, "FechaUtc");
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            e.Row.Cells[0].Text = TimeZoneInfo.ConvertTimeFromUtc(utc, tz).ToString("yyyy-MM-dd HH:mm");

            // Columna 3 = UserId -> lo traduzco a texto del ddl
            var userId = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "UserId"));
            var it = ddlUsuario.Items.FindByValue(userId);
            if (it != null && !string.IsNullOrEmpty(userId))
            {
                e.Row.Cells[3].Text = Server.HtmlEncode(it.Text);
            }
        }
    }

    void Buscar()
    {
        var _bll = new BLLBitacora();
        var filtro = new BEBitacoraFiltro
        {
            UserId = string.IsNullOrEmpty(ddlUsuario.SelectedValue) ? (int?)null : int.Parse(ddlUsuario.SelectedValue),
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

    
}

