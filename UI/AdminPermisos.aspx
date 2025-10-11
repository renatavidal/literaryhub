<%@ Page Title="Permisos y Administradores" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="AdminPermisos.aspx.cs" Inherits="AdminPermisos" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <style>
    .grid{display:grid;grid-template-columns:280px 1fr;gap:16px}
    .card{background:#fff;border:1px solid #e5e7eb;border-radius:12px;padding:12px}
    .list{max-height:420px;overflow:auto}
    .pill{display:inline-block;padding:4px 10px;border-radius:999px;background:#f3f4f6}
    .btn{padding:8px 12px;border:1px solid #e5e7eb;border-radius:8px;background:#f9fafb}
  </style>

  <h2>Permisos y Administradores</h2>
  <div class="grid">
    <!-- Roles -->
    <div class="card">
      <h3>Roles</h3>
      <asp:ListBox ID="lstRoles" runat="server" CssClass="list" AutoPostBack="true"
                   DataTextField="Nombre" DataValueField="Id" OnSelectedIndexChanged="lstRoles_SelectedIndexChanged" />
    </div>

    <!-- Permisos del rol -->
    <div class="card">
      <h3>Permisos asignados al rol <span class="pill"><asp:Literal ID="litRole" runat="server"/></span></h3>
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

  <div class="card" style="margin-top:16px">
    <h3>Administradores</h3>
    <div style="display:flex;gap:8px;align-items:center;flex-wrap:wrap">
      <asp:TextBox ID="txtUserId" runat="server" placeholder="UserId"></asp:TextBox>
      <asp:DropDownList ID="ddlAdminRole" runat="server" />
      <asp:Button ID="btnPromote" runat="server" CssClass="btn" Text="Promover" OnClick="btnPromote_Click" />
      <asp:Button ID="btnDemote"  runat="server" CssClass="btn" Text="Quitar rol" OnClick="btnDemote_Click" />
      <asp:Label ID="lblMsg" runat="server" />
    </div>
    <asp:GridView ID="gvAdmins" runat="server" AutoGenerateColumns="true" CssClass="table" GridLines="None" />
  </div>
</asp:Content>
