<%@ Page Title="Admin Finanzas" Language="C#" MasterPageFile="~/site.master" AutoEventWireup="true" CodeFile="AdminFinanzas.aspx.cs" Inherits="AdminFinanzas" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .wrap{display:grid;gap:18px;max-width:1040px}
    .card{background:var(--paper);border:1px solid var(--stroke);border-radius:14px;padding:16px}
    .grid2{display:grid;grid-template-columns:1fr 1fr;gap:12px}
    .row{display:grid;gap:6px}
    .input{padding:10px;border:1px solid #efe7df;border-radius:10px;background:#fff}
    .btn{padding:10px 14px;border-radius:10px;border:1px solid #b89567;background:#c9a97a;color:#281c0f;cursor:pointer}
    .hint{color:#7a6b5e}
    .table{width:100%;border-collapse:collapse}
    .table th,.table td{border-bottom:1px solid #eee;padding:8px;text-align:left}
    .right{text-align:right}
  </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="wrap">
    <h1>Finanzas: Notas y Cuenta Corriente</h1>

    <!-- Selección de usuario -->
    <div class="card">
      <div class="grid2">
        <div class="row">
          <label>Usuario</label>
          <asp:DropDownList ID="ddlUser" runat="server" CssClass="input" AutoPostBack="true" OnSelectedIndexChanged="ddlUser_SelectedIndexChanged" />
        </div>
        <div class="row">
          <label>Saldo Cta Cte (USD)</label>
          <asp:TextBox ID="txtSaldo" runat="server" CssClass="input" ReadOnly="true" />
        </div>
      </div>
      <asp:Literal ID="litInfo" runat="server" />
    </div>

    <!-- Alta de Nota -->
    <div class="card">
      <h3>Nueva Nota</h3>
      <div class="grid2">
        <div class="row">
          <label>Tipo</label>
          <asp:DropDownList ID="ddlNoteType" runat="server" CssClass="input">
            <asp:ListItem Text="Crédito (NC)" Value="C" Selected="True" />
            <asp:ListItem Text="Débito (ND)"  Value="D" />
          </asp:DropDownList>
        </div>
        <div class="row">
          <label>Monto (USD)</label>
          <asp:TextBox ID="txtNoteAmount" runat="server" CssClass="input" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="txtNoteAmount"
            ValidationExpression="^\d{1,7}(\.\d{1,2})?$" Display="Dynamic" CssClass="hint"
            ErrorMessage="Monto inválido (hasta 2 decimales)." />
        </div>
      </div>
      <div class="row" style="margin-top:8px">
        <label>Motivo</label>
        <asp:TextBox ID="txtNoteReason" runat="server" CssClass="input" MaxLength="120" />
      </div>
      <asp:Button ID="btnCrearNota" runat="server" Text="Crear Nota" CssClass="btn" OnClick="btnCrearNota_Click" />
    </div>

    <!-- Notas del usuario -->
    <div class="card">
      <h3>Notas del usuario</h3>
      <asp:GridView ID="gvNotas" runat="server"  DataKeyNames="Id" AutoGenerateColumns="false" CssClass="table" OnRowCommand="gvNotas_RowCommand">
        <Columns>
          <asp:BoundField DataField="Id" HeaderText="Id" />
          <asp:BoundField DataField="Number" HeaderText="Número" />
          <asp:BoundField DataField="Type" HeaderText="Tipo" />
          <asp:BoundField DataField="Amount" HeaderText="Importe" DataFormatString="{0:0.##}" />
          <asp:BoundField DataField="Remaining" HeaderText="Saldo" DataFormatString="{0:0.##}" />
          <asp:BoundField DataField="Reason" HeaderText="Motivo" />
          <asp:BoundField DataField="CreatedUtc" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
          <asp:ButtonField ButtonType="Button" CommandName="delNote" Text="Borrar" />
        </Columns>
      </asp:GridView>
    </div>

    <!-- Cuenta Corriente -->
    <div class="card">
      <h3>Cuenta Corriente (ABM)</h3>
      <div class="grid2">
        <div class="row">
          <label>Monto (positivo acredita / negativo debita)</label>
          <asp:TextBox ID="txtMovAmount" runat="server" CssClass="input" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="txtMovAmount"
            ValidationExpression="^-?\d{1,7}(\.\d{1,2})?$" Display="Dynamic" CssClass="hint"
            ErrorMessage="Monto inválido (use signo - para debitar)." />
        </div>
        <div class="row">
          <label>Concepto</label>
          <asp:TextBox ID="txtMovConcept" runat="server" CssClass="input" MaxLength="120" />
        </div>
      </div>
      <asp:Button ID="btnAgregarMov" runat="server" Text="Agregar movimiento" CssClass="btn" OnClick="btnAgregarMov_Click" />

      <asp:GridView ID="gvCuenta" runat="server" AutoGenerateColumns="false" CssClass="table" OnRowCommand="gvCuenta_RowCommand" style="margin-top:12px">
        <Columns>
          <asp:BoundField DataField="Id" HeaderText="Id" />
          <asp:BoundField DataField="Amount" HeaderText="Monto" DataFormatString="{0:0.##}" ItemStyle-CssClass="right" />
          <asp:BoundField DataField="Concept" HeaderText="Concepto" />
          <asp:BoundField DataField="CreatedUtc" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
          <asp:ButtonField ButtonType="Button" CommandName="delMov" Text="Borrar" />
        </Columns>
      </asp:GridView>
    </div>

    <asp:Literal ID="litMsg" runat="server" />
  </div>
</asp:Content>
