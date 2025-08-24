<%@ Page Title="Log in" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .auth { max-width: 520px; display:grid; gap:14px }
    .form-row{ display:grid; gap:6px }
    .input{ padding:10px; border:1px solid var(--stroke); border-radius:10px; background:var(--paper) }
    .row-inline{ display:flex; align-items:center; gap:8px; justify-content:space-between }
    .success{ color:green }
    .error{ color:maroon }
  </style>
</asp:Content>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Log in</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Iniciar sesión</h1>
  <p>Accedé a tus grupos de lectura, eventos y compras.</p>

  <asp:ValidationSummary ID="vsLogin" runat="server" ForeColor="Maroon" ValidationGroup="login" />

  <div class="auth">
    <!-- EMAIL -->
    <div class="form-row">
      <label for="txtEmail">Email</label>
      <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" MaxLength="120" />
      <asp:RequiredFieldValidator ID="reqEmail" runat="server"
        ControlToValidate="txtEmail" ErrorMessage="Ingresá tu email."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="login" />
      <asp:RegularExpressionValidator ID="revEmail" runat="server"
        ControlToValidate="txtEmail" ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
        ErrorMessage="Email no válido." Display="Dynamic" ValidationGroup="login" />
    </div>

    <!-- PASSWORD -->
    <div class="form-row">
      <label for="txtPassword">Contraseña</label>
      <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" />
      <asp:RequiredFieldValidator ID="reqPassword" runat="server"
        ControlToValidate="txtPassword" ErrorMessage="Ingresá tu contraseña."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="login" />
     
      <asp:RegularExpressionValidator ID="revPassword" runat="server"
        ControlToValidate="txtPassword" ValidationExpression="^.{8,128}$"
        ErrorMessage="La contraseña debe tener entre 8 y 128 caracteres."
        Display="Dynamic" ValidationGroup="login" />
    </div>

    <div class="row-inline">
      <a href="/Forgot.aspx">¿Olvidaste tu contraseña?</a>
    </div>

    <div>
            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="btn solid"
            OnClick="btnLogin_Click" UseSubmitBehavior="false" />
        <asp:Label ID="lblLoginResult" runat="server" />
    </div>

    <div>¿No tenés cuenta? <a href="~/Signup.aspx">Crear cuenta</a></div>
  </div>
</asp:Content>
