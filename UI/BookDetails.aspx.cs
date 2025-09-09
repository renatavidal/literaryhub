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
        var auth = Session["auth"] as UserSession;
        if (auth.IsInRole("Client") )
        {
            var alertMsg = (GetLocalResourceObject("Book_ClientAlert") as string) ?? "Como cliente no puedes comprar libros, crea una cuenta como usuario para disfrutar de la experiencia.";
            ScriptManager.RegisterStartupScript(this, GetType(), "err", "alert('" + alertMsg.Replace("'", "\\'") + "');", true);
            Response.Redirect("/Home.aspx");
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
            litStatus.Text = "<span style='color:green'>" + Server.HtmlEncode((GetLocalResourceObject("Book_StatusWantSuccess") as string) ?? "Marcado como \"Quiero leer\"!") + "</span>";
        }
        catch (Exception ex)
        {
            var __prefix = (GetLocalResourceObject("Book_StatusUpdateErrorPrefix") as string) ?? "No se pudo actualizar el estado. "; litStatus.Text = "<span style='color:#b00'>" + Server.HtmlEncode(__prefix + ex.Message) + "</span>";
        }
    }

    protected void btnRead_Click(object sender, EventArgs e)
    {
        try
        {
            var bll = new BLLCatalog();
            bll.SetUserBookStatus(CurrentUserId, BuildBookFromHidden(), UserBookStatus.Read);
            litStatus.Text = "<span style='color:green'>" + Server.HtmlEncode((GetLocalResourceObject("Book_StatusReadSuccess") as string) ?? "Marcado como \"Leído\"!") + "</span>";
        }
        catch (Exception ex)
        {
            var __prefix = (GetLocalResourceObject("Book_StatusUpdateErrorPrefix") as string) ?? "No se pudo actualizar el estado. "; litStatus.Text = "<span style='color:#b00'>" + Server.HtmlEncode(__prefix + ex.Message) + "</span>";
        }
    }

    protected void btnComment_Click(object sender, EventArgs e)
    {
        try
        {
            var text = (txtComment.Text ?? "").Trim();
            if (text.Length == 0)
            {
                litStatus.Text = "<span style='color:#b00'>" + Server.HtmlEncode((GetLocalResourceObject("Book_CommentEmpty") as string) ?? "El comentario no puede estar vacío.") + "</span>";
                return;
            }
            if (text.Length > 2000) text = text.Substring(0, 2000);

            var bll = new BLLCatalog();
            bll.AddComment(CurrentUserId, BuildBookFromHidden(), text);

            txtComment.Text = string.Empty;
            litStatus.Text = "<span style='color:green'>" + Server.HtmlEncode((GetLocalResourceObject("Book_CommentPublished") as string) ?? "¡Comentario publicado!") + "</span>";
        }
        catch (Exception ex)
        {
            var __prefix2 = (GetLocalResourceObject("Book_CommentPublishErrorPrefix") as string) ?? "No se pudo publicar el comentario. "; litStatus.Text = "<span style='color:#b00'>" + Server.HtmlEncode(__prefix2 + ex.Message) + "</span>";
        }
    }
}

