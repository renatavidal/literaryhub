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

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Crear cuenta</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Crear cuenta</h1>
  <p>Unite a LiteraryHub: descubrí libros, sumate a grupos y asistí a eventos con autores.</p>

  <asp:ValidationSummary ID="vsSignup" runat="server" ForeColor="Maroon" ValidationGroup="signup" />

  <div class="auth">
    <div class="form-2col">
      <!-- NOMBRE -->
      <div class="form-row">
        <label for="txtName">Nombre</label>
        <asp:TextBox ID="txtName" runat="server" CssClass="input" MaxLength="60" />
        <asp:RequiredFieldValidator ID="reqName" runat="server"
          ControlToValidate="txtName" ErrorMessage="Ingresá tu nombre."
          Display="Dynamic" SetFocusOnError="true" ValidationGroup="signup" />
        <asp:RegularExpressionValidator ID="revName" runat="server"
          ControlToValidate="txtName"
          ValidationExpression="^[A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- ']{2,60}$"
          ErrorMessage="Nombre inválido (solo letras y espacios, 2–60)."
          Display="Dynamic" ValidationGroup="signup" />
      </div>

      <!-- APELLIDO (opcional) -->
      <div class="form-row">
        <label for="txtLastName">Apellido (opcional)</label>
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
      <label for="txtEmail">Email</label>
      <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" MaxLength="120" />
      <asp:RequiredFieldValidator ID="reqEmail" runat="server"
        ControlToValidate="txtEmail" ErrorMessage="Ingresá tu email."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="signup" />
      <asp:RegularExpressionValidator ID="revEmail" runat="server"
        ControlToValidate="txtEmail" ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
        ErrorMessage="Email no válido." Display="Dynamic" ValidationGroup="signup" />
    </div>

    <!-- PASSWORD -->
    <div class="form-row">
      <label for="txtPassword">Contraseña</label>
      <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" MaxLength="64" />
      <span class="hint">Mín. 8 y máx. 64. Debe incluir: mayúscula, minúscula, número y símbolo.</span>
      <asp:RequiredFieldValidator ID="reqPassword" runat="server"
        ControlToValidate="txtPassword" ErrorMessage="Ingresá una contraseña."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="signup" />
      <!-- Complejidad: 1 mayúscula, 1 minúscula, 1 dígito, 1 símbolo; 8–64 -->
      <asp:RegularExpressionValidator ID="revPassword" runat="server"
        ControlToValidate="txtPassword"
        ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,64}$"
        ErrorMessage="La contraseña no cumple los requisitos."
        Display="Dynamic" ValidationGroup="signup" />
    </div>

    <!-- CONFIRM PASSWORD -->
    <div class="form-row">
      <label for="txtConfirm">Confirmar contraseña</label>
      <asp:TextBox ID="txtConfirm" runat="server" CssClass="input" TextMode="Password" MaxLength="64" />
      <asp:RequiredFieldValidator ID="reqConfirm" runat="server"
        ControlToValidate="txtConfirm" ErrorMessage="Repetí la contraseña."
        Display="Dynamic" SetFocusOnError="true" ValidationGroup="signup" />
      <asp:CompareValidator ID="cmpPasswords" runat="server"
        ControlToValidate="txtConfirm" ControlToCompare="txtPassword"
        ErrorMessage="Las contraseñas no coinciden."
        Display="Dynamic" ValidationGroup="signup" />
    </div>
      <div class="acceptance-sign-up">
             <div class="form-row">
          <div class="g-recaptcha"
               data-sitekey="<%= System.Configuration.ConfigurationManager.AppSettings["RecaptchaSiteKeyLocal"] %>"></div>

                  <asp:CustomValidator ID="cvCaptcha" runat="server"
                      OnServerValidate="cvCaptcha_ServerValidate"
                      ErrorMessage="Verificá que no sos un robot."
                      Display="Dynamic" ValidationGroup="login" />
                </div>

                <!-- TÉRMINOS -->
                <div class="form-row">
                    <div class="terminos-condiciones">
                         <asp:CheckBox ID="chkTerms" runat="server"
                           />
                        Acepto los
                         <a href="/Terms.aspx" target="_blank">Términos y condiciones</a> y
                         <a href="/Privacy.aspx" target="_blank">Privacidad</a>.
                         <asp:CustomValidator ID="cvTerms" runat="server"
                           OnServerValidate="cvTerms_ServerValidate"
                           ErrorMessage="Debés aceptar los Términos y la Privacidad."
                           Display="Dynamic" ValidationGroup="signup" />
                    </div>
                </div>  
        </div>
      

    <!-- BOTÓN -->
    <div>
      <asp:Button ID="btnSignup" runat="server" Text="Crear cuenta" CssClass="btn solid"
        OnClick="btnSignup_Click" ValidationGroup="signup" />
      <asp:Label ID="lblSignupResult" runat="server" />
    </div>

    <div>¿Ya tenés cuenta? <a href="/Login.aspx">Iniciar sesión</a></div>
  </div>
</asp:Content>
