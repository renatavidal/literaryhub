﻿<%@ Page Title="Publicidades (Admin)" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="AdsAdmin.aspx.cs" Inherits="AdsAdmin" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Publicidades</h2>

  <asp:Panel ID="pnlForm" runat="server" CssClass="card" DefaultButton="btnSave">
    <asp:HiddenField ID="hfId" runat="server" />
    <div class="grid-2" style="display:grid;grid-template-columns:1fr 1fr;gap:10px">
      <div>
        <label>Título</label>
        <asp:TextBox ID="txtTitle" runat="server" CssClass="input" />
      </div>
      <div>
        <label>Link</label>
        <asp:TextBox ID="txtLink" runat="server" CssClass="input" />
      </div>
      <div>
        <label>Imagen URL</label>
        <asp:TextBox ID="txtImg" runat="server" CssClass="input" />
      </div>
      <div>
        <label>Peso</label>
        <asp:TextBox ID="txtWeight" runat="server" CssClass="input" Text="1" />
      </div>
      <div>
        <label>Desde (UTC)</label>
        <input id="dtFrom" runat="server" type="datetime-local" />
      </div>
      <div>
        <label>Hasta (UTC)</label>
        <input id="dtTo" runat="server" type="datetime-local" />
      </div>
      <div style="display:flex;align-items:center;gap:8px">
        <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
        <span>Activo</span>
      </div>
    </div>

    <label style="margin-top:8px">Texto</label>
    <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="3" CssClass="input" />

    <div style="margin-top:10px;display:flex;gap:8px">
      <asp:Button ID="btnSave" runat="server" CssClass="btn" Text="Guardar" OnClick="btnSave_Click" />
      <asp:Button ID="btnNew" runat="server" CssClass="btn" Text="Nuevo" OnClick="btnNew_Click" CausesValidation="false" />
      <asp:Label ID="lblMsg" runat="server" />
    </div>
  </asp:Panel>

  <asp:Panel ID="pnlGrid" runat="server" CssClass="card" style="margin-top:14px">
    <h3>Listado</h3>
    <asp:GridView ID="gvAds" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
                  CssClass="table" OnRowCommand="gvAds_RowCommand">
      <Columns>
        <asp:BoundField DataField="Id" HeaderText="Id" />
        <asp:BoundField DataField="Title" HeaderText="Título" />
        <asp:CheckBoxField DataField="IsActive" HeaderText="Activo" />
        <asp:BoundField DataField="ImageUrl" HeaderText="Imagen" />
        <asp:BoundField DataField="LinkUrl" HeaderText="Link" />
        <asp:BoundField DataField="StartUtc" HeaderText="Desde (UTC)" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
        <asp:BoundField DataField="EndUtc" HeaderText="Hasta (UTC)" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
        <asp:TemplateField HeaderText="">
          <ItemTemplate>
            <asp:LinkButton runat="server" CommandName="EditAd" CommandArgument='<%# Eval("Id") %>' Text="Editar" />
            &nbsp;|&nbsp;
            <asp:LinkButton runat="server" CommandName="DelAd" CommandArgument='<%# Eval("Id") %>'
                            Text="Borrar" OnClientClick="return confirm('¿Borrar?');" />
          </ItemTemplate>
        </asp:TemplateField>
      </Columns>
    </asp:GridView>
  </asp:Panel>
</asp:Content>
