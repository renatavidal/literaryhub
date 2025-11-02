using System;
using System.Linq;
using System.Web.UI;
using BLL;
using BE;

public partial class AdsAdmin : Perm_AdminPublicidadesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) BindGrid();
    }

    private bool IsAdmin()
    {
        var auth = Session["auth"] as UserSession;
        if (auth == null) return false;
        var roles = auth.Roles ?? new string[0];
        return Array.Exists(roles, r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase));
    }

    private void BindGrid()
    {
        var list = new BLLAdvert().ListAll();
        gvAds.DataSource = list;
        gvAds.DataBind();
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        hfId.Value = "";
        txtTitle.Text = txtLink.Text = txtImg.Text = txtBody.Text = "";
        txtWeight.Text = "1";
        chkActive.Checked = true;
        dtFrom.Attributes["value"] = "";
        dtTo.Attributes["value"] = "";
        lblMsg.Text = "";
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            int id = 0; int.TryParse(hfId.Value, out id);
            int weight = 1; int.TryParse(txtWeight.Text, out weight);

            // lee <input type="datetime-local"> (string) y pasa a UTC
            DateTime? from = ParseLocal(dtFrom.Attributes["value"]);
            DateTime? to = ParseLocal(dtTo.Attributes["value"]);

            var ad = new BEAdvert
            {
                Id = id,
                Title = (txtTitle.Text ?? "").Trim(),
                Body = string.IsNullOrWhiteSpace(txtBody.Text) ? null : txtBody.Text.Trim(),
                ImageUrl = string.IsNullOrWhiteSpace(txtImg.Text) ? null : txtImg.Text.Trim(),
                LinkUrl = string.IsNullOrWhiteSpace(txtLink.Text) ? null : txtLink.Text.Trim(),
                IsActive = chkActive.Checked,
                Weight = weight < 1 ? 1 : weight,
                StartUtc = from,
                EndUtc = to
            };

            // ========== REGEX / VALIDACIONES ==========
            // Título: letras/números/espacios y puntuación básica
            var rxTitle = new System.Text.RegularExpressions.Regex(@"^[\p{L}\p{N}\s\.,;:'""\-!¡¿\?\(\)/_#&]+$");
            if (string.IsNullOrWhiteSpace(ad.Title) || !rxTitle.IsMatch(ad.Title))
                throw new Exception("Título inválido. Solo letras/números/espacios y puntuación básica.");

            // Descripción: máximo 50 chars
            if (!string.IsNullOrEmpty(ad.Body) && ad.Body.Length > 50)
                throw new Exception("La descripción no puede superar los 50 caracteres.");

            // URL absoluta http/https (si se cargan)
            var rxUrl = new System.Text.RegularExpressions.Regex(@"^https?:\/\/[^\s]+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (string.IsNullOrEmpty(ad.ImageUrl))
                throw new Exception("La ubicacion de la imagen debe ser válida.");
            if (!string.IsNullOrEmpty(ad.LinkUrl) && !rxUrl.IsMatch(ad.LinkUrl))
                throw new Exception("El link debe ser http(s) válido.");

            // Debe tener imagen o texto (al menos uno)
            if (string.IsNullOrEmpty(ad.ImageUrl) && string.IsNullOrEmpty(ad.Body))
                throw new Exception("Debe cargar imagen o texto.");

            // Fechas: inicio < fin; fin no en el pasado (UTC)
            if (ad.StartUtc.HasValue && ad.EndUtc.HasValue && ad.StartUtc.Value >= ad.EndUtc.Value)
                throw new Exception("La fecha de inicio debe ser anterior a la fecha de fin.");
            if (ad.EndUtc.HasValue && ad.EndUtc.Value < DateTime.UtcNow)
                throw new Exception("La fecha de fin no puede estar en el pasado.");

            // ==========================================

            int newId = new BLLAdvert().Save(ad);
            hfId.Value = newId.ToString();
            lblMsg.Text = "Guardado " + newId;
            BindGrid();
        }
        catch (Exception ex)
        {
            lblMsg.Text = "Error: " + ex.Message;
        }
    }


    protected void gvAds_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        int id; if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

        if (e.CommandName == "DelAd")
        {
            try { new BLLAdvert().Delete(id); BindGrid(); lblMsg.Text = "Eliminado."; }
            catch (Exception ex) { lblMsg.Text = "Error: " + ex.Message; }
        }
        else if (e.CommandName == "EditAd")
        {
            // Para simplificar, tomo desde el listado
            var list = new BLLAdvert().ListAll();
            var a = list.FirstOrDefault(x => x.Id == id);
            if (a == null) return;

            hfId.Value = a.Id.ToString();
            txtTitle.Text = a.Title;
            txtLink.Text = a.LinkUrl ?? "";
            txtImg.Text = a.ImageUrl ?? "";
            txtBody.Text = a.Body ?? "";
            txtWeight.Text = a.Weight.ToString();
            chkActive.Checked = a.IsActive;

            dtFrom.Attributes["value"] = a.StartUtc.HasValue ? a.StartUtc.Value.ToString("yyyy-MM-ddTHH:mm") : "";
            dtTo.Attributes["value"] = a.EndUtc.HasValue ? a.EndUtc.Value.ToString("yyyy-MM-ddTHH:mm") : "";
        }
    }

    // Convierte valor de <input type="datetime-local"> a UTC (DateTime?):
    private static DateTime? ParseLocal(string v)
    {
        if (string.IsNullOrWhiteSpace(v)) return null;
        DateTime local;
        if (DateTime.TryParse(v, out local))
            return DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();
        return null;
    }
}
