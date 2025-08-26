<%@ Page Title="Administrar Suscripciones" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="SuscripcionesAdmin.aspx.cs" Inherits="SuscripcionesAdmin" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .gv{
        overflow-x: scroll;
    }
    .admin-wrap{max-width:1100px}
    .gv td, .gv th{padding:8px 10px;
                           overflow-x: scroll;

    }
  </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Administrar Suscripciones</h1>
  <asp:Label ID="lblMsg" runat="server" />

  <asp:GridView ID="gvPlanes" runat="server" CssClass="gv"
      AutoGenerateColumns="False" DataKeyNames="Id"
      OnRowEditing="gv_RowEditing"
      OnRowCancelingEdit="gv_RowCancelingEdit"
      OnRowUpdating="gv_RowUpdating"
      OnRowDeleting="gv_RowDeleting"
      OnRowCommand="gv_RowCommand"
      ShowFooter="true">
    <Columns>
      <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="true" />
      <asp:TemplateField HeaderText="Código">
        <ItemTemplate><%# Eval("Codigo") %></ItemTemplate>
        <EditItemTemplate><asp:TextBox ID="txtCodigo" runat="server" Text='<%# Bind("Codigo") %>' MaxLength="30" /></EditItemTemplate>
        <FooterTemplate><asp:TextBox ID="ftCodigo" runat="server" MaxLength="30" /></FooterTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Descripción">
        <ItemTemplate><%# Eval("Descripcion") %></ItemTemplate>
        <EditItemTemplate><asp:TextBox ID="txtDesc" runat="server" Text='<%# Bind("Descripcion") %>' MaxLength="120" Width="220" /></EditItemTemplate>
        <FooterTemplate><asp:TextBox ID="ftDesc" runat="server" MaxLength="120" Width="220" /></FooterTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Roles (CSV)">
        <ItemTemplate><%# Eval("Roles") %></ItemTemplate>
        <EditItemTemplate><asp:TextBox ID="txtRoles" runat="server" Text='<%# Bind("Roles") %>' MaxLength="200" Width="200" /></EditItemTemplate>
        <FooterTemplate><asp:TextBox ID="ftRoles" runat="server" MaxLength="200" Width="200" /></FooterTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Precio USD">
        <ItemTemplate>$<%# Eval("PrecioUSD","{0:0.##}") %></ItemTemplate>
        <EditItemTemplate><asp:TextBox ID="txtPrecio" runat="server" Text='<%# Bind("PrecioUSD","{0:0.##}") %>' Width="80" /></EditItemTemplate>
        <FooterTemplate><asp:TextBox ID="ftPrecio" runat="server" Width="80" Text="0" /></FooterTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Orden">
        <ItemTemplate><%# Eval("Orden") %></ItemTemplate>
        <EditItemTemplate><asp:TextBox ID="txtOrden" runat="server" Text='<%# Bind("Orden") %>' Width="60" /></EditItemTemplate>
        <FooterTemplate><asp:TextBox ID="ftOrden" runat="server" Width="60" Text="0" /></FooterTemplate>
      </asp:TemplateField>
      <asp:CheckBoxField DataField="EsDestacado" HeaderText="Destacado" />
      <asp:CheckBoxField DataField="Activo" HeaderText="Activo" />
      <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" />
      <asp:TemplateField>
        <FooterTemplate>
          <asp:Button ID="btnInsert" runat="server" Text="Agregar" CommandName="Insert" CssClass="btn solid" />
        </FooterTemplate>
      </asp:TemplateField>
    </Columns>
  </asp:GridView>
</asp:Content>
