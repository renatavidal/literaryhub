using System;
using System.IO;
using BLL;

public partial class NewsletterAdmin : Perm_AdminNewsletterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Solo Admin
        var auth = Session["auth"] as UserSession;
        if (auth == null )
        {
            Response.Redirect("~/AccessDenied.aspx");
            return;
        }

        if (!IsPostBack) BindGrid();
    }

    private void BindGrid()
    {
        var bll = new BLLNewsletter();
        gvNews.DataSource = bll.ListarTodo();
        gvNews.DataBind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            var auth = Session["auth"] as UserSession;

            string imageUrl = null;
            if (fuImg.HasFile)
            {
                var ext = Path.GetExtension(fuImg.FileName).ToLowerInvariant();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp")
                    throw new InvalidOperationException("Formato de imagen no soportado.");

                var dir = Server.MapPath("~/Content/news");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var file = Guid.NewGuid().ToString("N") + ext;
                fuImg.SaveAs(Path.Combine(dir, file));
                imageUrl = "/Content/news/" + file;
            }

            var bll = new BLLNewsletter();
            int id = bll.Crear(auth.UserId,
                               (txtTitle.Text ?? "").Trim(),
                               (txtShort.Text ?? "").Trim(),
                               (txtFull.Text ?? "").Trim(),
                               imageUrl,
                               true);

            litMsg.Text = "<span style='color:green'>Guardado (ID " + id + ").</span>";
            txtTitle.Text = txtShort.Text = txtFull.Text = string.Empty;
            BindGrid();
        }
        catch (Exception ex)
        {
            litMsg.Text = "<span style='color:#b00'>Error: " + Server.HtmlEncode(ex.Message) + "</span>";
        }
    }

    protected void gvNews_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "del")
        {
            int id;
            if (int.TryParse(e.CommandArgument.ToString(), out id))
            {
                var bll = new BLLNewsletter();
                bll.Eliminar(id);
                BindGrid();
            }
        }
    }
}
