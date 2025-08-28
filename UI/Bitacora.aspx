<%@ Page Title="Bitácora" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Bitacora.aspx.cs" Inherits="Bitacora" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .filters{display:grid;grid-template-columns:repeat(5,1fr);gap:10px;margin-bottom:12px}
    @media(max-width:900px){.filters{grid-template-columns:1fr}}
    .input{padding:8px;border:1px solid var(--stroke);border-radius:10px;background:var(--paper)}
    .grid{width:100%;border-collapse:collapse}
    .grid th,.grid td{padding:8px;border-bottom:1px solid var(--stroke)}
    .muted{color:var(--ink-soft)}
  </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Bitácora</h1>

  <div class="filters">
    <div>
      <label>Desde (UTC)</label>
      <asp:TextBox ID="txtDesde" runat="server" CssClass="input" TextMode="Date" />
    </div>
    <div>
      <label>Hasta (UTC)</label>
      <asp:TextBox ID="txtHasta" runat="server" CssClass="input" TextMode="Date" />
    </div>
    <div>
      <label>Usuario</label>
      <asp:DropDownList ID="ddlUsuario" runat="server" CssClass="input">
        <asp:ListItem Text="Todos" Value=""></asp:ListItem>
      </asp:DropDownList>
    </div>
    <div>
      <label>Texto</label>
      <asp:TextBox ID="txtTexto" runat="server" CssClass="input" placeholder="Buscar en descripción..." />
    </div>
  </div>

  <div style="display:flex;gap:8px;margin-bottom:12px">
    <asp:Button ID="btnFiltrar" runat="server" CssClass="btn solid" Text="Filtrar" OnClick="btnFiltrar_Click" />
    <asp:Button ID="btnLimpiar" runat="server" CssClass="btn ghost" Text="Limpiar" OnClick="btnLimpiar_Click" />
    <span class="muted">Registros: <asp:Label ID="lblTotal" runat="server" Text="0" /></span>
  </div>

  <asp:GridView ID="gvBitacora" runat="server" CssClass="grid"
      AutoGenerateColumns="False" AllowPaging="True" PageSize="50"
      OnPageIndexChanging="gvBitacora_PageIndexChanging" OnRowDataBound="gvBitacora_RowDataBound">
    <Columns>
      <asp:BoundField DataField="FechaUtc" HeaderText="Fecha (local)" />
      <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
      <asp:BoundField DataField="Agente" HeaderText="Agente" />
      <asp:BoundField DataField="UserId" HeaderText="UserId" />
    </Columns>
  </asp:GridView>
</asp:Content>
