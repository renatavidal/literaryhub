<%@ Page Title="Backups (Admin)" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="BackupsAdmin.aspx.cs" Inherits="BackupsAdmin" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Backups</h2>

  <div class="card" style="margin-bottom:14px">
    <div style="display:flex;gap:10px;align-items:center;flex-wrap:wrap">
      <label>Nombre / etiqueta:
        <asp:TextBox ID="txtLabel" runat="server" CssClass="input" />
      </label>
      <asp:Button ID="btnCreate" runat="server" Text="Crear backup" CssClass="btn"
                  OnClick="btnCreate_Click" />
      <asp:Label ID="lblMsg" runat="server" />
    </div>
    <small>Se guarda en <code><%: System.Configuration.ConfigurationManager.AppSettings["BackupFolder"] %></code></small>
  </div>

  <div class="card">
    <h3>Listado</h3>
    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
                  OnRowCommand="gv_RowCommand" CssClass="table">
      <Columns>
        <asp:BoundField DataField="Id" HeaderText="Id" />
        <asp:BoundField DataField="Label" HeaderText="Etiqueta" />
        <asp:BoundField DataField="CreatedUtc" HeaderText="Fecha (UTC)" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
        <asp:BoundField DataField="FilePath" HeaderText="Archivo" />
        <asp:BoundField DataField="SizeBytes" HeaderText="Tamaño (bytes)" />
        <asp:TemplateField HeaderText="">
          <ItemTemplate>
            <asp:LinkButton runat="server" CommandName="restore" CommandArgument='<%# Eval("Id") %>'
              Text="Restore" OnClientClick="return confirm('Esto restaurará la base y cortará conexiones. ¿Continuar?');" />
          </ItemTemplate>
        </asp:TemplateField>
      </Columns>
    </asp:GridView>
  </div>
</asp:Content>
