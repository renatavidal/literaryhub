<%@ Page Title="Sign up" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Signup.aspx.cs" Inherits="Signup" %>


<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .auth { max-width: 620px; display:grid; gap:14px }
    .form-2col{ display:grid; grid-template-columns:1fr 1fr; gap:12px }
    @media(max-width:680px){ .form-2col{ grid-template-columns:1fr } }
    .form-row{ display:grid; gap:6px }
    .input{ padding:10px; border:1px solid var(--stroke); border-radius:10px; background:var(--paper) }
    .hint{ color:var(--ink-soft); font-size:.9rem }
    .success{ color:green } .error{ color:maroon }
    .g-recaptcha{ display:block; min-height:78px }
    .g-recaptcha > div{ width:100% }
    .acceptance-sign-up{
        display:flex;
        flex-direction:column;
        padding: 0.5rem;
        justify-content: space-between;
        align-items:center;
    }
    .terminos-condiciones{
        padding: 2rem;
        display: flex;
        flex-direction:row;
        justify-content: space-between;
        align-items:center;
    }
  </style>
</asp:Content>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server"><%: GetLocalResourceObject("Signup_Title") %></asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: GetLocalResourceObject("Signup_Title") %></h1>
  <p><%: GetLocalResourceObject("Signup_Intro") %></p>

  <asp:ValidationSummary ID="vsSignup" runat="server" ForeColor="Maroon" ValidationGroup="signup" />

  <div class="auth">
    <div class="form-2col">
      <!-- NOMBRE -->
      <div class="form-row">
        <label for="txtName"><%: GetLocalResourceObject("Signup_Label_Name") %></label>
        <asp:TextBox ID="txtName" runat="server" CssClass="input" MaxLength="60" />
        <asp:RequiredFieldValidator ID="reqName" runat="server"
          ControlToValidate="txtName" ErrorMessage="<%$ Resources: Signup_Name_Required %>"
          Display="Dynamic" SetFocusOnError="true" ValidationGroup="signup" />
        <asp:RegularExpressionValidator ID="revName" runat="server"
          ControlToValidate="txtName"
          ValidationExpression="^[A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- ']{2,60}$"
          ErrorMessage="Nombre inválido (solo letras y espacios, 2–60)."
          Display="Dynamic" ValidationGroup="signup" />
      </div>

      <!-- APELLIDO (opcional) -->
      <div class="form-row">
        <label for="txtLastName"><%: GetLocalResourceObject("Signup_Label_LastName") %></label>
        <asp:TextBox ID="txtLastName" runat="server" CssClass="input" MaxLength="60" />
        <asp:RegularExpressionValidator ID="revLastName" runat="server"
          ControlToValidate="txtLastName"
          ValidationExpression="^$|^[A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- ']{2,60}$"
          ErrorMessage="Apellido inválido (solo letras y espacios, 2–60)."
          Display="Dynamic" ValidationGroup="signup" />
      </div>
    </div>

    <!-- EMAIL -->
    <div class="form-row">
      <label for="txtEmail"><%: GetLocalResourceObject("Signup_Label_Email") %></label>
      <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" MaxLength="120" />
      <asp:RequiredFieldValidator ID="reqEmail" runat="server"
        ControlToValidate="txtEmail" ErrorMessage="<%$ Resources: Signup_Email_Required %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="signup" />
      <asp:RegularExpressionValidator ID="revEmail" runat="server"
        ControlToValidate="txtEmail" ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
        ErrorMessage="<%$ Resources: Signup_Email_Invalid %>" Display="Dynamic" ValidationGroup="signup" />
    </div>

    <!-- PASSWORD -->
    <div class="form-row">
      <label for="txtPassword"><%: GetLocalResourceObject("Signup_Label_Password") %></label>
      <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" MaxLength="64" />
      <span class="hint"><%: GetLocalResourceObject("Signup_Password_Hint") %></span>
      <asp:RequiredFieldValidator ID="reqPassword" runat="server"
        ControlToValidate="txtPassword" ErrorMessage="<%$ Resources: Signup_Password_Required %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="signup" />
      <!-- Complejidad: 1 mayúscula, 1 minúscula, 1 dígito, 1 símbolo; 8–64 -->
      <asp:RegularExpressionValidator ID="revPassword" runat="server"
        ControlToValidate="txtPassword"
        ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,64}$"
        ErrorMessage="<%$ Resources: Signup_Password_Invalid %>"
        Display="Dynamic" ValidationGroup="signup" />
    </div>

    <!-- CONFIRM PASSWORD -->
    <div class="form-row">
      <label for="txtConfirm"><%: GetLocalResourceObject("Signup_Label_Confirm") %></label>
      <asp:TextBox ID="txtConfirm" runat="server" CssClass="input" TextMode="Password" MaxLength="64" />
      <asp:RequiredFieldValidator ID="reqConfirm" runat="server"
        ControlToValidate="txtConfirm" ErrorMessage="<%$ Resources: Signup_Confirm_Required %>"
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="signup" />
      <asp:CompareValidator ID="cmpPasswords" runat="server"
        ControlToValidate="txtConfirm" ControlToCompare="txtPassword"
        ErrorMessage="<%$ Resources: Signup_Confirm_Invalid %>"
        Display="Dynamic" ValidationGroup="signup" />
    </div>
      <div class="acceptance-sign-up">
             <div class="form-row">
          <div class="g-recaptcha"
               data-sitekey="<%= System.Configuration.ConfigurationManager.AppSettings["RecaptchaSiteKey"] %>"></div>

                  <asp:CustomValidator ID="cvCaptcha" runat="server"
                      OnServerValidate="cvCaptcha_ServerValidate"
                      ErrorMessage="<%$ Resources: Signup_Captcha_Robot %>"
                      Display="Dynamic" ValidationGroup="login" />
                </div>

                <!-- TÉRMINOS -->
                <div class="form-row">
                    <div class="terminos-condiciones">
                         <asp:CheckBox ID="chkTerms" runat="server"
                           /><%: GetLocalResourceObject("Signup_Terms_Prefix") %> <a href="/Terms.aspx" target="_blank"><%: GetLocalResourceObject("Signup_Terms_LinkTerms") %></a> y
                         <a href="/Privacy.aspx" target="_blank"><%: GetLocalResourceObject("Signup_Terms_LinkPrivacy") %></a>.
                         <asp:CustomValidator ID="cvTerms" runat="server"
                           OnServerValidate="cvTerms_ServerValidate"
                           ErrorMessage="<%$ Resources: Signup_Terms_Error %>"
                           Display="Dynamic" ValidationGroup="signup" />
                    </div>
                </div>  
        </div>
      

    <!-- BOTÓN -->
    <div>
      <asp:Button ID="btnSignup" runat="server" Text="<%$ Resources: Signup_Submit %>" CssClass="btn solid"
        OnClick="btnSignup_Click" ValidationGroup="signup" />
      <asp:Label ID="lblSignupResult" runat="server" />
    </div>

    <div>¿Ya tenés cuenta? <a href="/Login.aspx"><%: GetLocalResourceObject("Signup_LoginLink") %></a></div>
  </div>
    <script src="https://www.google.com/recaptcha/api.js?hl=es" async defer></script>
</asp:Content>


