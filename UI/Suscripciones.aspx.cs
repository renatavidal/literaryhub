using System;
using BLL;

public partial class Suscripciones : ReaderPage  
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var bll = new BLLUsuario();
            var planes = bll.ListarPlanesSuscripcion();
            rptPlanes.DataSource = planes;
            rptPlanes.DataBind();
        }
    }
}
