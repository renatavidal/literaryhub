using System;
using System.Data;
using System.Web.UI.WebControls; // <-- por ListItem
using BLL;

public partial class AdminPermisos : Perm_AdminPermisos
{
    private readonly BLLPermissions _bll = new BLLPermissions();

    protected override bool RequireLogin { get { return true; } }
    protected override bool RequireVerifiedEmail { get { return true; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lstRoles.DataSource = _bll.Roles();
            lstRoles.DataBind();

            ddlAdminRole.DataSource = _bll.Roles();
            ddlAdminRole.DataTextField = "Nombre";
            ddlAdminRole.DataValueField = "Id";
            ddlAdminRole.DataBind();

            BindUsers(null);    
            BindAdmins();
            if (lstRoles.Items.Count > 0) lstRoles.SelectedIndex = 0;
            BindPerms();
        }
    }


    void BindPerms()
    {
        if (lstRoles.SelectedItem == null) { repPerms.DataSource = null; repPerms.DataBind(); return; }
        int roleId = int.Parse(lstRoles.SelectedValue);
        litRole.Text = lstRoles.SelectedItem.Text;
        repPerms.DataSource = _bll.RolePerms(roleId);
        repPerms.DataBind();
    }

    void BindAdmins()
    {
        gvAdmins.DataSource = _bll.AdminUsers();
        gvAdmins.DataBind();
    }

    void BindUsers(string q = null)
    {
        var dt = string.IsNullOrWhiteSpace(q)
            ? _bll.Users_Listar()         // TODOS
            : _bll.Usuarios_Buscar(q);    // FILTRADO

        // Armar NombreCompleto si no viene
        if (!dt.Columns.Contains("NombreCompleto"))
        {
            dt.Columns.Add("NombreCompleto", typeof(string));
            foreach (System.Data.DataRow r in dt.Rows)
            {
                var nombre = Convert.ToString(r["Nombre"]);
                var apellido = Convert.ToString(r["Apellido"]);
                r["NombreCompleto"] = nombre + " " +apellido;
            }
        }

        ddlUser.DataSource = dt;
        ddlUser.DataTextField = "NombreCompleto";
        ddlUser.DataValueField = "Id";
        ddlUser.DataBind();

        ddlUser.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Elegí usuario --", ""));
    }

    protected void btnBuscarUsuario_Click(object sender, EventArgs e)
    {
        BindUsers(txtBuscarUsuario.Text);
        lblMsg.Text = "Resultados filtrados.";
    }

    protected void btnLimpiarUsuario_Click(object sender, EventArgs e)
    {
        txtBuscarUsuario.Text = "";
        BindUsers(null); // TODOS
        lblMsg.Text = "Mostrando todos los usuarios.";
    }

    protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtUserId.Text = ddlUser.SelectedValue;
        lblMsg.Text = string.IsNullOrEmpty(txtUserId.Text)
            ? "Seleccioná un usuario."
            : "Usuario seleccionado: " + ddlUser.SelectedItem.Text;
    }


    // ====== NUEVO: crear rol ======
    protected void btnCrearRol_Click(object sender, EventArgs e)
    {
        try
        {
            var nombre = (txtNuevoRol.Text ?? "").Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                lblRolMsg.Text = "Ingresá un nombre para el rol.";
                return;
            }

            // Usa el método real de tu BLL (CreateRole / InsertRole / etc.)
            int newId = _bll.CreateRole(nombre); // ← Ajustar si tu método tiene otro nombre

            lblRolMsg.Text = "Rol creado (Id=" + newId + ").";
            txtNuevoRol.Text = "";

            // Refrescar listas de roles (izquierda y ddl para admins)
            lstRoles.DataSource = _bll.Roles(); lstRoles.DataBind();
            ddlAdminRole.DataSource = _bll.Roles(); ddlAdminRole.DataTextField = "Nombre"; ddlAdminRole.DataValueField = "Id"; ddlAdminRole.DataBind();

            if (lstRoles.Items.Count > 0)
            {
                // Seleccionar el recién creado si querés (opcional)
                var li = lstRoles.Items.FindByValue(newId.ToString());
                if (li != null) { lstRoles.ClearSelection(); li.Selected = true; }
                BindPerms();
            }
        }
        catch (Exception ex)
        {
            lblRolMsg.Text = ex.Message;
        }
    }

    protected void lstRoles_SelectedIndexChanged(object sender, EventArgs e) { BindPerms(); }

    protected void repPerms_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if (lstRoles.SelectedItem == null) return;
        int roleId = int.Parse(lstRoles.SelectedValue);
        int permId = int.Parse(Convert.ToString(e.CommandArgument));
        if (e.CommandName == "Grant") _bll.Grant(roleId, permId);
        if (e.CommandName == "Revoke") _bll.Revoke(roleId, permId);
        BindPerms();
    }

    protected void btnPromote_Click(object sender, EventArgs e)
    {
        int userId, roleId;
        if (int.TryParse(txtUserId.Text, out userId) && int.TryParse(ddlAdminRole.SelectedValue, out roleId))
        { _bll.AdminGrantRole(userId, roleId); lblMsg.Text = "Rol asignado."; BindAdmins(); }
        else lblMsg.Text = "Valores inválidos.";
    }

    protected void btnDemote_Click(object sender, EventArgs e)
    {
        int userId, roleId;
        if (int.TryParse(txtUserId.Text, out userId) && int.TryParse(ddlAdminRole.SelectedValue, out roleId))
        { _bll.AdminRevokeRole(userId, roleId); lblMsg.Text = "Rol quitado."; BindAdmins(); }
        else lblMsg.Text = "Valores inválidos.";
    }
}
