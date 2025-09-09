using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BLL;
using BE;

public partial class AdministrarUsuarios : AdminPage
{
    protected override bool RequireLogin { get { return true; } }
    protected override bool RequireVerifiedEmail { get { return true; } }
    protected override string[] RequiredRoles { get { return new[] { "Admin" }; } } // si usás roles

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            gvUsuarios.DataSource = new List<BENombre>();
            gvUsuarios.DataBind();

            var ph = GetLocalResourceObject("AdminUsers_SearchPlaceholder") as string;
            var tb = FindControl("txtSearch") as System.Web.UI.WebControls.TextBox;
            if (tb != null && !string.IsNullOrEmpty(ph)) tb.Attributes["placeholder"] = ph;
        }
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        string txt = txtTexto.Text.Trim();
        var bll = new BLLUsuario();
        var data = bll.Buscar(txt);
        if (data.Count == 0) lblMsg.Text = "Sin resultados.";
        else lblMsg.Text = "";
        gvUsuarios.DataSource = data;
        gvUsuarios.DataBind();
    }

    protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id;
        if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

        string agente = Request.UserAgent ?? "AdministrarUsuarios";
        bool ok = false;
        var bll = new BLLUsuario();
        if (e.CommandName == "Alta") ok = bll.DarDeAlta(id, agente);
        if (e.CommandName == "Baja") ok = bll.DarDeBaja(id, agente);

        lblMsg.Text = ok ? "Operación realizada." : "No se pudo aplicar el cambio.";
        btnBuscar_Click(sender, e); // recargar
    }
}
