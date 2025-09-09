<%@ Page Title="Bitácora" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Bitacora.aspx.cs" Inherits="Bitacora" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
    /* ===== tokens (fallbacks por si no existen en site.css) ===== */
    :root{
      --paper:#ffffff; --surface:#fff;
      --stroke:#e8dacd; --stroke-strong:#d8c8b9;
      --ink:#3b2f2a; --ink-soft:#7a6b5e;
      --brand:#a47148; --brand-dark:#6b4226;
      --bg-soft:#f8f5f1;
    }

    /* ===== título ===== */
    h1{
      font-family: Georgia, serif;
      color: var(--ink);
      margin: 6px 0 18px;
      font-size: 28px;
      letter-spacing:.2px;
    }

    /* ===== contenedor de filtros como "card" ===== */
    .filters{
      display:grid;
      grid-template-columns:repeat(5,1fr);
      gap:14px;
      margin: 0 0 14px;
      padding:14px;
      background: var(--surface);
      border:1px solid var(--stroke);
      border-radius:14px;
      box-shadow: 0 8px 24px rgba(0,0,0,.06);
    }
    @media (max-width:1100px){ .filters{ grid-template-columns:repeat(3,1fr); } }
    @media (max-width:900px){ .filters{ grid-template-columns:1fr; } }

    .filters label{
      display:block;
      margin:0 0 6px;
      font-size:12px;
      color:var(--ink-soft);
      letter-spacing:.2px;
    }

    /* inputs tipo pill */
    .input{
      width:100%;
      height:42px;
      padding:10px 12px;
      border:1px solid var(--stroke);
      border-radius:12px;
      background:#fff;
      color:var(--ink);
      box-shadow: 0 1px 0 rgba(0,0,0,.02) inset;
      transition: border-color .15s ease, box-shadow .15s ease;
    }
    .input:focus{
      outline:none;
      border-color: var(--brand);
      box-shadow: 0 0 0 3px rgba(164,113,72,.15);
    }

    /* iconito de búsqueda en el texto libre */
    .filters input[type="text"].input{
      background-image:url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%237a6b5e' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Ccircle cx='11' cy='11' r='8'/%3E%3Cline x1='21' y1='21' x2='16.65' y2='16.65'/%3E%3C/svg%3E");
      background-repeat:no-repeat;
      background-position:10px 50%;
      padding-left:36px;
    }

    /* ===== botones ===== */
    .btn{ 
      appearance:none; border:0; border-radius:12px; height:40px;
      padding:0 14px; cursor:pointer; font-weight:600;
      transition:transform .04s ease, box-shadow .15s ease, background .15s ease, color .15s ease;
    }
    .btn:active{ transform: translateY(1px); }

    .btn.solid{
      background: var(--brand);
      color:#fff;
      box-shadow: 0 6px 14px rgba(164,113,72,.25);
    }
    .btn.solid:hover{ background: var(--brand-dark); }

    .btn.ghost{
      background: transparent;
      color: var(--brand-dark);
      border:1px solid var(--stroke-strong);
    }
    .btn.ghost:hover{
      border-color: var(--brand);
      color: var(--brand-dark);
      box-shadow: 0 0 0 3px rgba(164,113,72,.12);
    }

    .muted{ color:var(--ink-soft); align-self:center; }

    /* ===== tabla estilo "card" ===== */
    .grid{
      width:100%;
      border-collapse:separate;           /* para que funcione border-radius */
      border-spacing:0;
      background:#fff;
      border:1px solid var(--stroke);
      border-radius:14px;
      box-shadow:0 10px 28px rgba(0,0,0,.06);
      overflow:hidden;                    /* redondeo real */
    }
    .grid th, .grid td{
      padding:12px 14px;
      border-bottom:1px solid var(--stroke);
      font-size:14px;
      color:var(--ink);
      vertical-align:middle;
      white-space:nowrap;
    }
    .grid th{
      background:#f5f0ea;
      color:#5a4232;
      font-weight:700;
      position:sticky; top:0; z-index:1;  /* header fijo si hay scroll */
    }
    .grid tr:last-child td{ border-bottom:0; }

    /* zebra + hover */
    .grid tbody tr:nth-child(odd) td{ background: #fcfaf8; }
    .grid tbody tr:hover td{
      background: #f3eee8;
    }

    /* primero/último para ver mejor el borde redondeado */
    .grid thead tr:first-child th:first-child{ border-top-left-radius:14px; }
    .grid thead tr:first-child th:last-child{  border-top-right-radius:14px; }
    .grid tbody tr:last-child td:first-child{  border-bottom-left-radius:14px; }
    .grid tbody tr:last-child td:last-child{   border-bottom-right-radius:14px; }
  </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: GetLocalResourceObject("Audit_Title") %></h1>

  <div class="filters">
    <div>
      <label><%: GetLocalResourceObject("Audit_Filter_From") %> (UTC)</label>
      <asp:TextBox ID="txtDesde" runat="server" CssClass="input" TextMode="Date" />
    </div>
    <div>
      <label><%: GetLocalResourceObject("Audit_Filter_To") %> (UTC)</label>
      <asp:TextBox ID="txtHasta" runat="server" CssClass="input" TextMode="Date" />
    </div>
    <div>
      <label><%: GetLocalResourceObject("Audit_Filter_User") %></label>
      <asp:DropDownList ID="ddlUsuario" runat="server" CssClass="input">
        <asp:ListItem Text="<%$ Resources: Audit_Filter_All %>" Value=""></asp:ListItem>
      </asp:DropDownList>
    </div>
    <div>
      <label><%: GetLocalResourceObject("Audit_Filter_Action") %></label>
      <asp:TextBox ID="txtTexto" runat="server" CssClass="input" placeholder="Buscar en descripción..." />
    </div>
  </div>

  <div style="display:flex;gap:8px;margin-bottom:12px">
    <asp:Button ID="btnFiltrar" runat="server" CssClass="btn solid" Text="<%$ Resources: Audit_ApplyFilters %>" OnClick="btnFiltrar_Click" />
    <asp:Button ID="btnLimpiar" runat="server" CssClass="btn ghost" Text="<%$ Resources: Audit_ClearFilters %>" OnClick="btnLimpiar_Click" />
    <span class="muted"><%: GetLocalResourceObject("Audit_Records") %> <asp:Label ID="lblTotal" runat="server" Text="0" /></span>
  </div>

  <asp:GridView ID="gvBitacora" runat="server" CssClass="grid"
      AutoGenerateColumns="False" AllowPaging="True" PageSize="50"
      OnPageIndexChanging="gvBitacora_PageIndexChanging" OnRowDataBound="gvBitacora_RowDataBound">
    <Columns>
      <asp:BoundField DataField="FechaUtc" HeaderText="<%$ Resources: Audit_Col_Timestamp %>" />
      <asp:BoundField DataField="Descripcion" HeaderText="<%$ Resources: Audit_Col_Action %>" />
      <asp:BoundField DataField="Agente" HeaderText="<%$ Resources: Audit_Col_Agent %>" />
      <asp:BoundField DataField="UserId" HeaderText="<%$ Resources: Audit_Col_User %>" />
    </Columns>
  </asp:GridView>
</asp:Content>


