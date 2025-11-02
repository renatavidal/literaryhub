using System;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI;
using BLL;

public partial class AdminFinanzas : Perm_AdminFinanzasPage
{
    private readonly BLLFinanzasAdmin _bll = new BLLFinanzasAdmin();
    private readonly BLLUsuario _bllUsuario = new BLLUsuario();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarUsuarios();
            if (ddlUser.Items.Count > 0) RefrescarTodo();
        }
    }

    private void CargarUsuarios()
    {
        ddlUser.Items.Clear();
        var lista = _bllUsuario.ListarUsuariosParaFiltro();
        foreach (var it in lista)
            ddlUser.Items.Add(new System.Web.UI.WebControls.ListItem(it.Texto, it.Id.ToString()));
    }

    private int UserIdSel()
    {
        int id; return int.TryParse(ddlUser.SelectedValue, out id) ? id : 0;
    }

    private void RefrescarTodo()
    {
        int uid = UserIdSel();
        if (uid <= 0) return;

        // Saldo c/c
        txtSaldo.Text = _bll.SaldoCuenta(uid).ToString("0.##", CultureInfo.InvariantCulture);

        // Notas
        gvNotas.DataSource = _bll.NotasPorUsuario(uid);
        gvNotas.DataBind();

        // Cuenta Corriente
        gvCuenta.DataSource = _bll.CuentaPorUsuario(uid);
        gvCuenta.DataBind();
    }

    protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefrescarTodo();
    }

    protected void btnCrearNota_Click(object sender, EventArgs e)
    {
        int uid = UserIdSel(); if (uid <= 0) { litMsg.Text = "<div class='hint'>Elegí un usuario.</div>"; return; }

        decimal amount;
        if (!decimal.TryParse((txtNoteAmount.Text ?? "").Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out amount))
        { litMsg.Text = "<div class='hint'>Monto inválido.</div>"; return; }

        string reason = (txtNoteReason.Text ?? "").Trim();
        string type = ddlNoteType.SelectedValue;

        int noteId = (type == "D")
            ? _bll.CrearNotaDebito(uid, amount, reason)
            : _bll.CrearNotaCredito(uid, amount, reason);

        litMsg.Text = "<div class='hint'>Nota creada: #" + noteId + "</div>";
        txtNoteAmount.Text = "";
        txtNoteReason.Text = "";
        RefrescarTodo();
    }

    protected void gvNotas_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "delNote")
        {
            // Con ButtonField, CommandArgument = índice de la fila
            int rowIndex;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out rowIndex)) return;

            // DataKeys ya poblado por DataKeyNames="Id"
            int noteId = Convert.ToInt32(gvNotas.DataKeys[rowIndex].Value);

            if (_bll.BorrarNota(noteId))
                litMsg.Text = "<div class='hint'>Nota borrada.</div>";
            else
                litMsg.Text = "<div class='hint'>No se pudo borrar: ya aplicada o sin saldo completo.</div>";

            RefrescarTodo();
        }
    }


    protected void gvNotas_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
        {
            // habilitar DataKeys
            gvNotas.DataKeyNames = new[] { "Id" };
        }
    }

    protected void gvCuenta_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "delMov")
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            int movId = Convert.ToInt32(gvCuenta.DataKeys[rowIndex].Value);
            if (_bll.BorrarMovimientoCC(movId))
                litMsg.Text = "<div class='hint'>Movimiento borrado.</div>";
            else
                litMsg.Text = "<div class='hint'>No se pudo borrar.</div>";
            RefrescarTodo();
        }
    }

    protected void gvCuenta_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
        {
            gvCuenta.DataKeyNames = new[] { "Id" };
        }
    }

    protected void btnAgregarMov_Click(object sender, EventArgs e)
    {
        int uid = UserIdSel(); if (uid <= 0) return;

        decimal amount;
        if (!decimal.TryParse((txtMovAmount.Text ?? "").Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out amount))
        { litMsg.Text = "<div class='hint'>Monto inválido.</div>"; return; }

        string concept = (txtMovConcept.Text ?? "").Trim();
        if (string.IsNullOrEmpty(concept)) concept = "(ajuste)";

        if (_bll.AgregarMovimientoCC(uid, amount, concept))
            litMsg.Text = "<div class='hint'>Movimiento agregado.</div>";
        else
            litMsg.Text = "<div class='hint'>No se pudo agregar.</div>";

        txtMovAmount.Text = ""; txtMovConcept.Text = "";
        RefrescarTodo();
    }
}
