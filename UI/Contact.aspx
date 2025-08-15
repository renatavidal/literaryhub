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
  Contáctenos
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Contáctenos</h1>
  <p>¿Tenés una consulta, propuesta de alianza o prensa? Escribinos y te respondemos.</p>

  <asp:ValidationSummary ID="valSummary" runat="server" ForeColor="Maroon" ValidationGroup="contact" />

  <div class="form-grid">
    <!-- NOMBRE -->
    <div class="form-row">
      <label for="txtName">Nombre</label>
      <asp:TextBox ID="txtName" runat="server" CssClass="input" MaxLength="60" />
      <asp:RequiredFieldValidator ID="reqName" runat="server"
        ControlToValidate="txtName" ErrorMessage="Ingresá tu nombre."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
      <asp:RegularExpressionValidator ID="revName" runat="server"
        ControlToValidate="txtName"
        ValidationExpression="^[A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- ']{2,60}$"
        ErrorMessage="Nombre inválido (solo letras y espacios, 2–60)."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
    </div>

    <!-- EMAIL -->
    <div class="form-row">
      <label for="txtEmail">Email</label>
      <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" MaxLength="120" />
      <asp:RequiredFieldValidator ID="reqEmail" runat="server"
        ControlToValidate="txtEmail" ErrorMessage="Ingresá tu email."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
      <asp:RegularExpressionValidator ID="revEmail" runat="server"
        ControlToValidate="txtEmail"
        ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
        ErrorMessage="Email no válido."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
    </div>

    <!-- ASUNTO -->
    <div class="form-row">
      <label for="txtSubject">Asunto</label>
      <asp:TextBox ID="txtSubject" runat="server" CssClass="input" MaxLength="120" />
      <asp:RequiredFieldValidator ID="reqSubject" runat="server"
        ControlToValidate="txtSubject" ErrorMessage="Ingresá un asunto."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
      <asp:RegularExpressionValidator ID="revSubject" runat="server"
        ControlToValidate="txtSubject"
        ValidationExpression="^[A-Za-z0-9ÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ ,.;:¡!¿?\-\(\)'\/&%+\[\]]{3,120}$"
        ErrorMessage="Asunto inválido (3–120 caracteres, sin símbolos raros)."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
    </div>

    <!-- MENSAJE -->
    <div class="form-row">
      <label for="txtMessage">Mensaje</label>
      <asp:TextBox ID="txtMessage" runat="server" CssClass="textarea" TextMode="MultiLine" Rows="6" />
      <asp:RequiredFieldValidator ID="reqMessage" runat="server"
        ControlToValidate="txtMessage" ErrorMessage="Escribí tu mensaje."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
      <asp:RegularExpressionValidator ID="revMessage" runat="server"
        ControlToValidate="txtMessage"
        ValidationExpression="^(?=.*\S)[\s\S]{10,2000}$"
        ErrorMessage="Mensaje demasiado corto (mínimo 10 caracteres) o vacío."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="contact" />
    </div>

    <!-- BOTÓN -->
    <div>
      <asp:Button ID="btnSend" runat="server" Text="Enviar" CssClass="btn solid"
        OnClick="btnSend_Click" ValidationGroup="contact" />
      <asp:Label ID="lblResult" runat="server" CssClass="success" />
    </div>
  </div>
</asp:Content>
