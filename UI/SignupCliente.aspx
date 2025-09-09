<%@ Page Title="Alta de Cliente" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="SignupCliente.aspx.cs" Inherits="SignupCliente" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>.auth{max-width:780px;display:grid;gap:12px}.grid2{display:grid;grid-template-columns:1fr 1fr;gap:12px}
  .input{padding:10px;border:1px solid var(--stroke);border-radius:10px;background:var(--paper)}</style>
  <script src="https://www.google.com/recaptcha/api.js?hl=es" async defer></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: GetLocalResourceObject("SC_Title") %></h1>
  <asp:ValidationSummary runat="server" ValidationGroup="c" ForeColor="Maroon" />

  <div class="auth">
    <div class="grid2">
      <div>
        <label for="txtCuil"><%: GetLocalResourceObject("SC_Label_Cuil") %></label>
        <asp:TextBox ID="txtCuil" runat="server" CssClass="input" MaxLength="14" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCuil" ErrorMessage="<%$ Resources: SC_Cuil_Required %>" ValidationGroup="c" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtCuil"
          ValidationExpression="^\d{11}$|^\d{2}-\d{8}-\d$" ErrorMessage="<%$ Resources: SC_Cuil_Invalid %>" ValidationGroup="c" />
      </div>
      <div>
        <label for="txtEmail"><%: GetLocalResourceObject("SC_Label_Email") %></label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" MaxLength="120" />
         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
      ErrorMessage="<%$ Resources: SC_Email_Required %>" ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail"
      ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
      ErrorMessage="<%$ Resources: SC_Email_Invalid %>" ValidationGroup="c" />
      </div>
      <div>
        <label for="txtNombre"><%: GetLocalResourceObject("SC_Label_Name") %></label>
        <asp:TextBox ID="txtNombre" runat="server" CssClass="input" MaxLength="60" />
         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNombre"
              ErrorMessage="<%$ Resources: SC_Name_Required %>" ValidationGroup="c" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="txtNombre"
              ValidationExpression="^[A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- ']{2,60}$"
              ErrorMessage="Nombre inválido (solo letras/espacios, 2–60)."
              ValidationGroup="c" />
      </div>
      <div>
        <label for="txtApellido"><%: GetLocalResourceObject("SC_Label_LastName") %></label>
        <asp:TextBox ID="txtApellido" runat="server" CssClass="input" MaxLength="60" />
           <asp:RequiredFieldValidator runat="server" ControlToValidate="txtApellido"
      ErrorMessage="<%$ Resources: SC_LastName_Required %>" ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtApellido"
      ValidationExpression="^[A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- ']{2,60}$"
      ErrorMessage="Apellido inválido (solo letras/espacios, 2–60)."
      ValidationGroup="c" />
      </div>
      <div>
        <label for="txtAlias"><%: GetLocalResourceObject("SC_Label_Alias") %></label>
        <asp:TextBox ID="txtAlias" runat="server" CssClass="input" MaxLength="120" />
           <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAlias"
      ErrorMessage="<%$ Resources: SC_Alias_Required %>" ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtAlias"
      ValidationExpression="^[0-9A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- '()&/]{2,120}$"
      ErrorMessage="Alias inválido (2–120, letras/nros/.,-/()&/)."
      ValidationGroup="c" />
      </div>
      <div>
        <label for="txtTel"><%: GetLocalResourceObject("SC_Label_Tel") %></label>
        <asp:TextBox ID="txtTel" runat="server" CssClass="input" MaxLength="30" />
          <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTel"
      ErrorMessage="<%$ Resources: SC_Tel_Required %>" ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtTel"
      ValidationExpression="^[0-9()+\-.\s]{6,25}$"
      ErrorMessage="Teléfono inválido (6–25, dígitos + () - . espacios)."
      ValidationGroup="c" />
      </div>
        <div class="form-row">
          <label><%: GetLocalResourceObject("SC_Label_Tipo") %></label>
          <asp:RadioButtonList ID="rblTipo" runat="server" RepeatDirection="Horizontal"
                               CssClass="input" OnSelectedIndexChanged="rblTipo_SelectedIndexChanged" AutoPostBack="false">
            <asp:ListItem Text="<%$ Resources: SC_Tipo_Autor %>" Value="AUT" Selected="True" />
            <asp:ListItem Text="<%$ Resources: SC_Tipo_Libreria %>" Value="LIB" />
          </asp:RadioButtonList>
              <asp:RequiredFieldValidator runat="server" ControlToValidate="rblTipo"
      ErrorMessage="<%$ Resources: SC_Tipo_Required %>" ValidationGroup="c" />
        </div>

<div class="form-row" id="rowUbicacion" runat="server">
  <label for="txtUbicacion"><%: GetLocalResourceObject("SC_Label_Ubicacion") %></label>
  <asp:TextBox ID="txtUbicacion" runat="server" CssClass="input" MaxLength="200" />
  <!-- requerido solo si es librería -->
  <asp:CustomValidator ID="cvUbicacion" runat="server"
      ClientValidationFunction="valUbicacion"
      OnServerValidate="cvUbicacion_ServerValidate"
      ErrorMessage="Ingresá una ubicación para librería."
      Display="Dynamic" ValidationGroup="c" />
</div>
    </div>

    <h3>Datos de facturación</h3>
    <div class="grid2">
      <div>
        <label for="ddlIVA">Condición IVA</label>
        <asp:DropDownList ID="ddlIVA" runat="server" CssClass="input">
          <asp:ListItem Text="Consumidor Final" Value="CF" />
          <asp:ListItem Text="Monotributo" Value="MONO" />
          <asp:ListItem Text="Responsable Inscripto" Value="RI" />
          <asp:ListItem Text="Exento" Value="EX" />
        </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlIVA"
      InitialValue="" ErrorMessage="Seleccioná la condición de IVA."
      ValidationGroup="c" />
      </div>
      <div>
        <label for="txtRazSoc">Razón social</label>
        <asp:TextBox ID="txtRazSoc" runat="server" CssClass="input" MaxLength="120" />
           <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRazSoc"
      ErrorMessage="Ingresá la razón social." ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtRazSoc"
      ValidationExpression="^[A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ0-9\.\- '()&/]{2,120}$"
      ErrorMessage="Razón social inválida." ValidationGroup="c" />
      </div>
      <div>
        <label for="txtCuitFac">CUIT facturación</label>
        <asp:TextBox ID="txtCuitFac" runat="server" CssClass="input" MaxLength="14" />
           <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCuitFac"
      ErrorMessage="Ingresá el CUIT de facturación." ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtCuitFac"
      ValidationExpression="^\d{11}$|^\d{2}-\d{8}-\d$"
      ErrorMessage="CUIT inválido (11 dígitos o XX-XXXXXXXX-X)."
      ValidationGroup="c" />
      </div>
      <div>
        <label for="txtDomFac"><%: GetLocalResourceObject("SC_Label_DomFac") %></label>
        <asp:TextBox ID="txtDomFac" runat="server" CssClass="input" MaxLength="200" />
           <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDomFac"
      ErrorMessage="<%$ Resources: SC_DomFac_Required %>" ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtDomFac"
      ValidationExpression="^[^\r\n]{4,200}$"
      ErrorMessage="Domicilio inválido (4–200 caracteres, sin saltos de línea)."
      ValidationGroup="c" />
      </div>
      <div>
        <label for="txtEmailFac"><%: GetLocalResourceObject("SC_Label_EmailFac") %></label>
        <asp:TextBox ID="txtEmailFac" runat="server" CssClass="input" MaxLength="120" />
           <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmailFac"
      ErrorMessage="<%$ Resources: SC_EmailFac_Required %>" ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmailFac"
      ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
      ErrorMessage="<%$ Resources: SC_EmailFac_Invalid %>" ValidationGroup="c" />
      </div>
    </div>
      <!-- PASSWORD -->
<div class="form-row">
  <label for="txtPassword"><%: GetLocalResourceObject("SC_Password_Label") %></label>
  <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" MaxLength="64" />
  <span class="hint"><%: GetLocalResourceObject("SC_Password_Hint") %></span>
  <asp:RequiredFieldValidator ID="reqPassword" runat="server"
    ControlToValidate="txtPassword" ErrorMessage="<%$ Resources: SC_Password_Required %>"
    Display="Dynamic" SetFocusOnError="true" ValidationGroup="signup" />
  <!-- Complejidad: 1 mayúscula, 1 minúscula, 1 dígito, 1 símbolo; 8–64 -->
  <asp:RegularExpressionValidator ID="revPassword" runat="server"
    ControlToValidate="txtPassword"
    ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,64}$"
    ErrorMessage="<%$ Resources: SC_Password_Invalid %>"
    Display="Dynamic" ValidationGroup="signup" />
</div>

<!-- CONFIRM PASSWORD -->
<div class="form-row">
  <label for="txtConfirm"><%: GetLocalResourceObject("SC_Confirm_Label") %></label>
  <asp:TextBox ID="txtConfirm" runat="server" CssClass="input" TextMode="Password" MaxLength="64" />
  <asp:RequiredFieldValidator ID="reqConfirm" runat="server"
    ControlToValidate="txtConfirm" ErrorMessage="<%$ Resources: SC_Confirm_Required %>"
    Display="Dynamic" SetFocusOnError="true" ValidationGroup="signup" />
  <asp:CompareValidator ID="cmpPasswords" runat="server"
    ControlToValidate="txtConfirm" ControlToCompare="txtPassword"
    ErrorMessage="<%$ Resources: SC_Confirm_Mismatch %>"
    Display="Dynamic" ValidationGroup="signup" />
</div>
       <!-- TÉRMINOS -->
 <div class="form-row">
     <div class="terminos-condiciones">
          <asp:CheckBox ID="chkTerms" runat="server"
            />
         Acepto los
          <a href="/Terms.aspx" target="_blank"><%: GetLocalResourceObject("SC_Terms_LinkTerms") %></a> y
          <a href="/Privacy.aspx" target="_blank"><%: GetLocalResourceObject("SC_Terms_LinkPrivacy") %></a>.
          <asp:CustomValidator ID="cvTerms" runat="server"
            OnServerValidate="cvTerms_ServerValidate"
            ErrorMessage="<%$ Resources: SC_Terms_Error %>"
            Display="Dynamic" ValidationGroup="signup" />
     </div>
 </div>  

    <div class="g-recaptcha" data-sitekey="<%= System.Configuration.ConfigurationManager.AppSettings["RecaptchaSiteKey"] %>"></div>

    <div>
      <asp:Button ID="btnCrear" runat="server" Text="<%$ Resources: SC_Button_Create %>" CssClass="btn solid" OnClick="btnCrear_Click" ValidationGroup="c" />
      <asp:Label ID="lblResultado" runat="server" />
    </div>
  </div>
    <script>
        function toggleUbicacion() {
            var sel = document.querySelector('input[name="<%= rblTipo.UniqueID %>"]:checked');
      var row = document.getElementById('<%= rowUbicacion.ClientID %>');
            if (!sel) return;
            row.style.display = (sel.value === 'LIB') ? 'block' : 'none';
        }
        function valUbicacion(source, args) {
            var sel = document.querySelector('input[name="<%= rblTipo.UniqueID %>"]:checked');
    if (sel && sel.value === 'LIB') {
      args.IsValid = document.getElementById('<%= txtUbicacion.ClientID %>').value.trim().length > 0;
    } else {
      args.IsValid = true;
    }
  }
  document.addEventListener('change', function(e){
    if (e.target && e.target.name === '<%= rblTipo.UniqueID %>') toggleUbicacion();
  });
        document.addEventListener('DOMContentLoaded', toggleUbicacion);
    </script>
</asp:Content>


