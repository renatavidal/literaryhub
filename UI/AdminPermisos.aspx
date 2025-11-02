<%@ Page Title="Permisos y Administradores" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="AdminPermisos.aspx.cs" Inherits="AdminPermisos" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <style>
    .grid{display:grid;grid-template-columns:280px 1fr;gap:16px}
    .card{background:#fff;border:1px solid #e5e7eb;border-radius:12px;padding:12px}
    .list{max-height:420px;overflow:auto}
    .pill{display:inline-block;padding:4px 10px;border-radius:999px;background:#f3f4f6}
    .btn{padding:8px 12px;border:1px solid #e5e7eb;border-radius:8px;background:#f9fafb}
    .row{display:flex;gap:8px;align-items:center;flex-wrap:wrap;margin:8px 0}
    .hint{color:#b45309}
    .row-hide{visibility:hidden}
  </style>

  <h2>Permisos y Administradores</h2>

  <div class="grid">
    <!-- Roles -->
    <div class="card">
      <h3>Roles</h3>
      <asp:ListBox ID="lstRoles" runat="server" CssClass="list" AutoPostBack="true"
        DataTextField="Nombre" DataValueField="Id"
        OnSelectedIndexChanged="lstRoles_SelectedIndexChanged" />

      <!-- Alta de rol -->
      <div class="row">
        <asp:TextBox ID="txtNuevoRol" runat="server" placeholder="Nuevo rol (nombre)" MaxLength="64" />
        <asp:Button ID="btnCrearRol" runat="server" CssClass="btn" Text="Crear rol" OnClick="btnCrearRol_Click" />
        <asp:Label ID="lblRolMsg" runat="server" CssClass="hint" />
      </div>
    </div>

    <!-- Permisos del rol -->
    <div class="card">
      <h3>
        Permisos asignados al rol
        <span class="pill"><asp:Literal ID="litRole" runat="server" /></span>
      </h3>
      <asp:Repeater ID="repPerms" runat="server" OnItemCommand="repPerms_ItemCommand">
        <ItemTemplate>
          <div style="display:flex;justify-content:space-between;align-items:center;padding:6px 0;border-bottom:1px solid #f1f1f1">
            <div>
              <b><%# Eval("Key") %></b><br />
              <small><%# Eval("Name") %></small>
            </div>
            <asp:Button runat="server" CssClass="btn"
              CommandName='<%# (Convert.ToInt32(Eval("Granted"))==1) ? "Revoke" : "Grant" %>'
              CommandArgument='<%# Eval("Id") %>'
              Text='<%# (Convert.ToInt32(Eval("Granted"))==1) ? "Revocar" : "Otorgar" %>' />
          </div>
        </ItemTemplate>
      </asp:Repeater>
    </div>
  </div>

  <!-- Administradores -->
  <div class="card" style="margin-top:16px">
    <h3>Usuarios</h3>

    <!-- Selección de usuario por dropdown -->
    <div class="row-hide">
    <asp:TextBox ID="txtBuscarUsuario" runat="server" placeholder="Buscar nombre, apellido o email" />
    <asp:Button ID="btnBuscarUsuario" runat="server" CssClass="btn" Text="Buscar" OnClick="btnBuscarUsuario_Click" />
    <asp:Button ID="btnLimpiarUsuario" runat="server" CssClass="btn" Text="Ver todos" OnClick="btnLimpiarUsuario_Click" />
  </div>

  <div class="row">
    <asp:DropDownList ID="ddlUser" runat="server" AutoPostBack="true"
        OnSelectedIndexChanged="ddlUser_SelectedIndexChanged" />
    <!-- Este textbox solo muestra el Id para conservar compatibilidad con tus validaciones -->
    <asp:TextBox ID="txtUserId" runat="server" ReadOnly="true" placeholder="UserId" MaxLength="10" />
    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtUserId"
        ValidationExpression="^\d+$" CssClass="hint" Display="Dynamic"
        ErrorMessage="UserId inválido." />
    <asp:DropDownList ID="ddlAdminRole" runat="server" />
    <asp:Button ID="btnPromote" runat="server" CssClass="btn" Text="Promover" OnClick="btnPromote_Click" />
    <asp:Button ID="btnDemote" runat="server" CssClass="btn" Text="Quitar rol" OnClick="btnDemote_Click" />
    <asp:Label ID="lblMsg" runat="server" CssClass="hint" />
  </div>

  <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="true" CssClass="table" GridLines="None" />
</div>

    <!-- Listado de admins/roles actuales -->
    <asp:GridView ID="gvAdmins" runat="server" AutoGenerateColumns="true" CssClass="table" GridLines="None" />
 
</asp:Content>
