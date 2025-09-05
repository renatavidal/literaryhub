using System;

public partial class Purchase : ReaderPage
{
    protected override bool RequireLogin { get { return true; } }
    protected override bool RequireVerifiedEmail { get { return true; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        // nada: el UserControl se encarga
    }
}
