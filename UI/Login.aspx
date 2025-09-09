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

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server"><%: GetLocalResourceObject("Login_Title") %></asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: GetLocalResourceObject("Login_Title") %></h1>
  <p><%: GetLocalResourceObject("Login_Intro") %></p>

  <asp:ValidationSummary ID="vsLogin" runat="server" ForeColor="Maroon" ValidationGroup="login" />

  <div class="auth">
    <!-- EMAIL -->
    <div class="form-row">
      <label for="txtEmail"><%: GetLocalResourceObject("Login_Label_Email") %></label>
      <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" MaxLength="120" />
      <asp:RequiredFieldValidator ID="reqEmail" runat="server"
        ControlToValidate="txtEmail" ErrorMessage="<%$ Resources: Login_Email_Required %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="login" />
      <asp:RegularExpressionValidator ID="revEmail" runat="server"
        ControlToValidate="txtEmail" ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
        ErrorMessage="<%$ Resources: Login_Email_Invalid %>" Display="Dynamic" ValidationGroup="login" />
    </div>

    <!-- PASSWORD -->
    <div class="form-row">
      <label for="txtPassword"><%: GetLocalResourceObject("Login_Label_Password") %></label>
      <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" />
      <asp:RequiredFieldValidator ID="reqPassword" runat="server"
        ControlToValidate="txtPassword" ErrorMessage="<%$ Resources: Login_Password_Required %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="login" />
     
      <asp:RegularExpressionValidator ID="revPassword" runat="server"
        ControlToValidate="txtPassword" ValidationExpression="^.{8,128}$"
        ErrorMessage="<%$ Resources: Login_Password_Invalid %>"
        Display="Dynamic" ValidationGroup="login" />
    </div>

    <div class="row-inline">
      <a href="/Forgot.aspx"><%: GetLocalResourceObject("Login_Forgot") %></a>
    </div>

    <div>
            <asp:Button ID="btnLogin" runat="server" Text="<%$ Resources: Login_Submit %>" CssClass="btn solid"
            OnClick="btnLogin_Click" UseSubmitBehavior="false" />
        <asp:Label ID="lblLoginResult" runat="server" />
    </div>

    <div>¿No tenés cuenta? <a href="/Signup.aspx"><%: GetLocalResourceObject("Login_CreateAccount") %></a></div>
  </div>
</asp:Content>


