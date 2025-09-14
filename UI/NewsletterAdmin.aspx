<%@ Page Title="Administrar Newsletter" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="NewsletterAdmin.aspx.cs" Inherits="NewsletterAdmin" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .form{display:grid;gap:12px;max-width:900px}
    .row{display:grid;gap:6px}
    .input{padding:10px;border:1px solid var(--stroke);border-radius:10px;background:#fff}
    .btn{padding:10px 14px;border-radius:10px;border:1px solid var(--brand);background:var(--brand);color:#fff;cursor:pointer}
    .grid{width:100%;border-collapse:collapse;margin-top:18px}
    .grid th,.grid td{padding:8px;border-bottom:1px solid var(--stroke);vertical-align:top}
    .thumb{width:90px;height:60px;object-fit:cover;border-radius:8px;background:#fbf8f4}
    .muted{color:#7a6b5e}
  </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Administrar Newsletter</h1>
  <p class="muted">Crear nuevas noticias o eliminar anteriores.</p>

  <div class="form">
    <div class="row">
      <label for="<%=txtTitle.ClientID%>">Título</label>
      <asp:TextBox ID="txtTitle" runat="server" CssClass="input" MaxLength="200" />
     <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTitle"
    ValidationGroup="nl" Display="Dynamic" CssClass="hint"
    ErrorMessage="Ingresá el título." />
<asp:RegularExpressionValidator runat="server" ControlToValidate="txtTitle"
    ValidationGroup="nl" Display="Dynamic" CssClass="hint"
    ValidationExpression="^[\s\S]{1,50}$"
    ErrorMessage="Máximo 50 caracteres." />
    </div>

    <div class="row">
      <label for="<%=txtShort.ClientID%>">Short description</label>
      <asp:TextBox ID="txtShort" runat="server" CssClass="input" TextMode="MultiLine" Rows="3" MaxLength="400" />
     <asp:RequiredFieldValidator runat="server" ControlToValidate="txtShort"
    ValidationGroup="nl" Display="Dynamic" CssClass="hint"
    ErrorMessage="Ingresá la descripción corta." />
<asp:RegularExpressionValidator runat="server" ControlToValidate="txtShort"
    ValidationGroup="nl" Display="Dynamic" CssClass="hint"
    ValidationExpression="^[\s\S]{0,400}$"
    ErrorMessage="Máximo 400 caracteres." />
    </div>

    <div class="row">
     <asp:TextBox ID="txtFull" runat="server" CssClass="input"
             TextMode="MultiLine" Rows="6" MaxLength="400" />
<asp:RequiredFieldValidator runat="server" ControlToValidate="txtFull"
    ValidationGroup="nl" Display="Dynamic" CssClass="hint"
    ErrorMessage="Ingresá la descripción completa." />
<asp:RegularExpressionValidator runat="server" ControlToValidate="txtFull"
    ValidationGroup="nl" Display="Dynamic" CssClass="hint"
    ValidationExpression="^[\s\S]{0,400}$"
    ErrorMessage="Máximo 400 caracteres." />
    </div>

    <div class="row">
      <label for="<%=fuImg.ClientID%>">Imagen (jpg/png/webp)</label>
      <asp:FileUpload ID="fuImg" runat="server" CssClass="input" />
    </div>


    <asp:Button ID="btnSave" runat="server" CssClass="btn" Text="Guardar" OnClick="btnSave_Click" />
    <asp:Literal ID="litMsg" runat="server" />
  </div>

  <asp:GridView ID="gvNews" runat="server" CssClass="grid" AutoGenerateColumns="False"
      OnRowCommand="gvNews_RowCommand">
    <Columns>
      <asp:TemplateField HeaderText="Imagen">
        <ItemTemplate>
          <img class="thumb" alt="" src='<%# string.IsNullOrEmpty(Eval("ImageUrl") as string) ? "/Content/blank-cover.png" : Eval("ImageUrl") %>' />
        </ItemTemplate>
      </asp:TemplateField>
      <asp:BoundField DataField="Title" HeaderText="Título" />
      <asp:BoundField DataField="ShortDescription" HeaderText="Short" />
      <asp:BoundField DataField="CreatedUtc" HeaderText="Fecha (UTC)" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
      <asp:TemplateField HeaderText="Acciones">
        <ItemTemplate>
          <asp:LinkButton runat="server" CommandName="del" CommandArgument='<%# Eval("Id") %>' OnClientClick="return confirm('¿Eliminar?');">Eliminar</asp:LinkButton>
        </ItemTemplate>
      </asp:TemplateField>
    </Columns>
  </asp:GridView>
</asp:Content>
