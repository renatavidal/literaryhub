<%@ Page Title="Contáctenos" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Contact.aspx.cs" Inherits="Contact" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .form-grid{display:grid; gap:12px; max-width:720px}
    .form-row{display:grid; gap:6px}
    .input, .textarea{padding:10px; border:1px solid var(--stroke); border-radius:10px; background:var(--paper)}
    .success{color:green; margin-top:8px}
    .photo-grid{display:grid; grid-template-columns:1fr; gap:12px}
    .photo-slot{height:140px; border:2px dashed var(--stroke); border-radius:12px; display:flex; align-items:center; justify-content:center; color:var(--ink-soft)}
  </style>
</asp:Content>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">
  <%: GetLocalResourceObject("Contact_Title") %>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: GetLocalResourceObject("Contact_Title") %></h1>
  <p><%: GetLocalResourceObject("Contact_Intro") %></p>

  <asp:ValidationSummary ID="valSummary" runat="server" ForeColor="Maroon" ValidationGroup="contact" />

  <div class="form-grid">
    <!-- NOMBRE -->
    <div class="form-row">
      <label for="txtName"><%: GetLocalResourceObject("Contact_Label_Name") %></label>
      <asp:TextBox ID="txtName" runat="server" CssClass="input" MaxLength="60" />
      <asp:RequiredFieldValidator ID="reqName" runat="server"
        ControlToValidate="txtName" ErrorMessage="<%$ Resources: Contact_Name_Required %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
      <asp:RegularExpressionValidator ID="revName" runat="server"
        ControlToValidate="txtName"
        ValidationExpression="^[A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- ']{2,60}$"
        ErrorMessage="Nombre inválido (solo letras y espacios, 2–60)."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
    </div>

    <!-- EMAIL -->
    <div class="form-row">
      <label for="txtEmail"><%: GetLocalResourceObject("Contact_Label_Email") %></label>
      <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" MaxLength="120" />
      <asp:RequiredFieldValidator ID="reqEmail" runat="server"
        ControlToValidate="txtEmail" ErrorMessage="<%$ Resources: Contact_Email_Required %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
      <asp:RegularExpressionValidator ID="revEmail" runat="server"
        ControlToValidate="txtEmail"
        ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
        ErrorMessage="<%$ Resources: Contact_Email_Invalid %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
    </div>

    <!-- ASUNTO -->
    <div class="form-row">
      <label for="txtSubject"><%: GetLocalResourceObject("Contact_Label_Subject") %></label>
      <asp:TextBox ID="txtSubject" runat="server" CssClass="input" MaxLength="120" />
      <asp:RequiredFieldValidator ID="reqSubject" runat="server"
        ControlToValidate="txtSubject" ErrorMessage="<%$ Resources: Contact_Subject_Required %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
      <asp:RegularExpressionValidator ID="revSubject" runat="server"
        ControlToValidate="txtSubject"
        ValidationExpression="^[A-Za-z0-9ÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ ,.;:¡!¿?\-\(\)'\/&%+\[\]]{3,120}$"
        ErrorMessage="Asunto inválido (3–120 caracteres, sin símbolos raros)."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
    </div>

    <!-- MENSAJE -->
    <div class="form-row">
      <label for="txtMessage"><%: GetLocalResourceObject("Contact_Label_Message") %></label>
      <asp:TextBox ID="txtMessage" runat="server" CssClass="textarea" TextMode="MultiLine" Rows="6" />
      <asp:RequiredFieldValidator ID="reqMessage" runat="server"
        ControlToValidate="txtMessage" ErrorMessage="<%$ Resources: Contact_Message_Required %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
      <asp:RegularExpressionValidator ID="revMessage" runat="server"
        ControlToValidate="txtMessage"
        ValidationExpression="^(?=.*\S)[\s\S]{10,2000}$"
        ErrorMessage="<%$ Resources: Contact_Message_Invalid %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
    </div>

    <!-- BOTÓN -->
    <div>
      <asp:Button ID="btnSend" runat="server" Text="<%$ Resources: Contact_Send %>" CssClass="btn solid"
        OnClick="btnSend_Click" ValidationGroup="contact" />
      <asp:Label ID="lblResult" runat="server" CssClass="success" />
    </div>
  </div>
</asp:Content>


