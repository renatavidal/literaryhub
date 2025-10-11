using System;
using System.Linq;
using System.Web.UI;
using BLL;

public partial class BackupsAdmin : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsAdmin()) { Response.Redirect("~/AccessDenied.aspx"); return; }
        if (!IsPostBack) Bind();
    }

    private bool IsAdmin()
    {
        var auth = Session["auth"] as UserSession;
        if (auth == null) return false;
        var roles = auth.Roles ?? new string[0];
        return Array.Exists(roles, r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase));
    }

    private void Bind()
    {
        gv.DataSource = new BLLBackup().List();
        gv.DataBind();
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        try
        {
            var b = new BLLBackup().Create(txtLabel.Text);
            lblMsg.Text = "Backup creado en: " + b.FilePath;
            Bind();
        }
        catch (Exception ex)
        {
            lblMsg.Text = "Error: " + ex.Message;
        }
    }

    protected void gv_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "restore")
        {
            int id; if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            try
            {
                // IMPORTANTE: esto corta todas las conexiones (incluida esta).
                new BLLBackup().Restore(id);
                // Si llega acá, el restore terminó (posible que esta respuesta no se entregue).
                lblMsg.Text = "Restore completado.";
            }
            catch (Exception ex) { lblMsg.Text = "Error: " + ex.Message; }
        }
    }
}
