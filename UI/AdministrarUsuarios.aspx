<%@ Page Title="Administrar Usuarios" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="AdministrarUsuarios.aspx.cs" Inherits="AdministrarUsuarios" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .admin { max-width:980px; margin:24px auto; display:grid; gap:14px }
    .card { background:#fff; border:1px solid var(--stroke); border-radius:14px; padding:16px }
    .row { display:grid; grid-template-columns:160px 1fr auto; gap:10px; align-items:center }
    .btn{ padding:8px 12px; border:1px solid var(--stroke); border-radius:10px; background:var(--bg-soft) }
    .btn-danger{ background:#fbeaea; border-color:#f2c2c2 }
    .muted{ color:#666 }
    .table{ width:100% }
  </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="admin">
    <h1><%: GetLocalResourceObject("AdminUsers_Title") %></h1>

    <div class="card">
      <div class="row">
        <label for="txtTexto">Nombre / Apellido / Email</label>
        <asp:TextBox ID="txtTexto" runat="server" CssClass="input" />
        <asp:Button ID="btnBuscar" runat="server" Text="<%$ Resources: AdminUsers_Search %>" CssClass="btn" OnClick="btnBuscar_Click" />
      </div>
      <asp:Label ID="lblMsg" runat="server" CssClass="muted" />
    </div>

    <div class="card">
      <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False" CssClass="table"
                    OnRowCommand="gvUsuarios_RowCommand">
        <Columns>
          <asp:BoundField HeaderText="ID" DataField="Id" />
          <asp:BoundField HeaderText="<%$ Resources: AdminUsers_Name %>" DataField="Nombre" />
          <asp:BoundField HeaderText="<%$ Resources: AdminUsers_LastName %>" DataField="Apellido" />
          <asp:BoundField HeaderText="<%$ Resources: AdminUsers_Email %>" DataField="Email" />
          <asp:CheckBoxField HeaderText="<%$ Resources: AdminUsers_Verified %>" DataField="EmailVerified" />
          <asp:CheckBoxField HeaderText="<%$ Resources: AdminUsers_Enabled %>" DataField="Activo" />
          <asp:TemplateField HeaderText="<%$ Resources: AdminUsers_Actions %>">
            <ItemTemplate>
              <asp:Button runat="server" Text="<%$ Resources: AdminUsers_Edit %>" CommandName="Alta" CommandArgument='<%# Eval("Id") %>' CssClass="btn" />
              <asp:Button runat="server" Text="<%$ Resources: AdminUsers_Delete %>" CommandName="Baja" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-danger"
                          OnClientClick="return confirm('<%$ Resources: AdminUsers_ConfirmDelete %>');" />
            </ItemTemplate>
          </asp:TemplateField>
        </Columns>
      </asp:GridView>
    </div>
  </div>
</asp:Content>

