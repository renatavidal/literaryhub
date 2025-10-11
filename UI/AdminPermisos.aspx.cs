using System;
using System.Data;
using BLL;

public partial class AdminPermisos : AdminPage
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
            ddlAdminRole.DataSource = _bll.Roles(); ddlAdminRole.DataTextField = "Nombre"; ddlAdminRole.DataValueField = "Id"; ddlAdminRole.DataBind();
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
