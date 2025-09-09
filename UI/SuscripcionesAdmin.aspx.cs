using System;
using System.Globalization;
using BLL;
using BE;

public partial class SuscripcionesAdmin : System.Web.UI.Page
{
    BLLUsuario _bll = new BLLUsuario();
    protected UserSession CurrentUser
    {
        get
        {
            return (Session != null) ? (Session["auth"] as UserSession) : null;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        var u = CurrentUser;
        bool esAdmin = u != null && u.IsInRole("Admin");
        if (!esAdmin)
        {
            Response.Redirect("/AccessDenied.aspx");
            return;
        }

        if (!IsPostBack) BindGV();
    }

    void BindGV()
    {
        gvPlanes.DataSource = _bll.ListarPlanesAdmin();
        gvPlanes.DataBind();
    }

    protected void gv_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
    { gvPlanes.EditIndex = e.NewEditIndex; BindGV(); }

    protected void gv_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
    { gvPlanes.EditIndex = -1; BindGV(); }

    protected void gv_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
    {
        var row = gvPlanes.Rows[e.RowIndex];
        int id = (int)gvPlanes.DataKeys[e.RowIndex].Value;

        var s = new BESuscripcion
        {
            Id = id,
            Codigo = ((System.Web.UI.WebControls.TextBox)row.FindControl("txtCodigo")).Text.Trim(),
            Descripcion = ((System.Web.UI.WebControls.TextBox)row.FindControl("txtDesc")).Text.Trim(),
            Roles = ((System.Web.UI.WebControls.TextBox)row.FindControl("txtRoles")).Text.Trim(),
            PrecioUSD = decimal.Parse(((System.Web.UI.WebControls.TextBox)row.FindControl("txtPrecio")).Text, CultureInfo.InvariantCulture),
            Orden = int.Parse(((System.Web.UI.WebControls.TextBox)row.FindControl("txtOrden")).Text),
            EsDestacado = ((System.Web.UI.WebControls.CheckBox)row.Cells[6].Controls[0]).Checked,
            Activo = ((System.Web.UI.WebControls.CheckBox)row.Cells[7].Controls[0]).Checked
        };

        try
        {
            _bll.ActualizarPlan(s);
            gvPlanes.EditIndex = -1;
            BindGV();
            lblMsg.CssClass = "success"; lblMsg.Text = (GetLocalResourceObject("SA_UpdateSuccess") as string) ?? "Plan actualizado.";
        }
        catch (Exception ex)
        {
            lblMsg.CssClass = "error"; lblMsg.Text = ex.Message;
        }
    }

    protected void gv_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
    {
        int id = (int)gvPlanes.DataKeys[e.RowIndex].Value;
        try
        {
            _bll.ArchivarPlan(id);
            BindGV();
            lblMsg.CssClass = "success"; lblMsg.Text = (GetLocalResourceObject("SA_DeactivateSuccess") as string) ?? "Plan desactivado.";
        }
        catch (Exception ex)
        {
            lblMsg.CssClass = "error"; lblMsg.Text = ex.Message;
        }
    }

    protected void gv_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Insert")
        {
            var ft = gvPlanes.FooterRow;
            var s = new BESuscripcion
            {
                Codigo = ((System.Web.UI.WebControls.TextBox)ft.FindControl("ftCodigo")).Text.Trim(),
                Descripcion = ((System.Web.UI.WebControls.TextBox)ft.FindControl("ftDesc")).Text.Trim(),
                Roles = ((System.Web.UI.WebControls.TextBox)ft.FindControl("ftRoles")).Text.Trim(),
                PrecioUSD = decimal.Parse(((System.Web.UI.WebControls.TextBox)ft.FindControl("ftPrecio")).Text, CultureInfo.InvariantCulture),
                Orden = int.Parse(((System.Web.UI.WebControls.TextBox)ft.FindControl("ftOrden")).Text),
                EsDestacado = false,
                Activo = true
            };
            try
            {
                _bll.CrearPlan(s);
                BindGV();
                lblMsg.CssClass = "success"; lblMsg.Text = (GetLocalResourceObject("SA_CreateSuccess") as string) ?? "Plan creado.";
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "error"; lblMsg.Text = ex.Message;
            }
        }
    }
}

