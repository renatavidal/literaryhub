using System;
using System.Web;
using System.Web.UI;
using BE;
using BLL;

public partial class BookDetails : ReaderPage 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var gid = Request["gid"];
        if (string.IsNullOrWhiteSpace(gid))
        {
            Response.Redirect("/AccessDenied.aspx"); 
            return;
        }

        if (!IsPostBack)
        {
        }
    }

    private int CurrentUserId
    {
        get
        {
            var auth = Session["auth"] as UserSession; 
            if (auth == null) 
            {
                var ret = Server.UrlEncode(Request.RawUrl);
                Response.Redirect("/Login.aspx?returnUrl=" + ret);
                return 0;
            }
            return auth.UserId;
        }
    }

    private BEBook BuildBookFromHidden()
    {
        return new BEBook
        {
            GoogleVolumeId = hidGid.Value,
            Title = hidTitle.Value,
            Authors = hidAuthors.Value,
            ThumbnailUrl = hidThumb.Value,
            Isbn13 = hidIsbn13.Value,
            PublishedDate = hidPub.Value
        };
    }

    protected void btnWant_Click(object sender, EventArgs e)
    {
        try
        {
            var bll = new BLLCatalog();
            bll.SetUserBookStatus(CurrentUserId, BuildBookFromHidden(), UserBookStatus.WantToRead);
            litStatus.Text = "<span style='color:green'>¡Marcado como “Quiero leer”!</span>";
        }
        catch (Exception ex)
        {
            litStatus.Text = "<span style='color:#b00'>No se pudo actualizar el estado. " + Server.HtmlEncode(ex.Message) + "</span>";
        }
    }

    protected void btnRead_Click(object sender, EventArgs e)
    {
        try
        {
            var bll = new BLLCatalog();
            bll.SetUserBookStatus(CurrentUserId, BuildBookFromHidden(), UserBookStatus.Read);
            litStatus.Text = "<span style='color:green'>¡Marcado como “Leído”!</span>";
        }
        catch (Exception ex)
        {
            litStatus.Text = "<span style='color:#b00'>No se pudo actualizar el estado. " + Server.HtmlEncode(ex.Message) + "</span>";
        }
    }

    protected void btnComment_Click(object sender, EventArgs e)
    {
        try
        {
            var text = (txtComment.Text ?? "").Trim();
            if (text.Length == 0)
            {
                litStatus.Text = "<span style='color:#b00'>El comentario no puede estar vacío.</span>";
                return;
            }
            if (text.Length > 2000) text = text.Substring(0, 2000);

            var bll = new BLLCatalog();
            bll.AddComment(CurrentUserId, BuildBookFromHidden(), text);

            txtComment.Text = string.Empty;
            litStatus.Text = "<span style='color:green'>¡Comentario publicado!</span>";
        }
        catch (Exception ex)
        {
            litStatus.Text = "<span style='color:#b00'>No se pudo publicar el comentario. " + Server.HtmlEncode(ex.Message) + "</span>";
        }
    }
}
