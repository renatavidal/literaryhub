using System;
using BLL;

public partial class Newsletter : ReaderPage
{
    private const int PageSize = 10;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        int pageIndex = 0; int tmp;
        if (int.TryParse(Request["p"], out tmp) && tmp >= 0) pageIndex = tmp;

        var bll = new BLLNewsletter();
        int total;
        var list = bll.ListarPublicados(pageIndex, PageSize, out total);

        rptNews.DataSource = list;
        rptNews.DataBind();

        lnkPrev.Visible = pageIndex > 0;
        if (lnkPrev.Visible) lnkPrev.NavigateUrl = "?p=" + (pageIndex - 1);

        bool hasMore = (pageIndex + 1) * PageSize < total;
        lnkNext.Visible = hasMore;
        if (hasMore) lnkNext.NavigateUrl = "?p=" + (pageIndex + 1);

        litTotal.Text = "Total: " +  total.ToString();
    }
}
