<%@ Page Title="Alta de Cliente" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="SignupCliente.aspx.cs" Inherits="SignupCliente" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>.auth{max-width:780px;display:grid;gap:12px}.grid2{display:grid;grid-template-columns:1fr 1fr;gap:12px}
  .input{padding:10px;border:1px solid var(--stroke);border-radius:10px;background:var(--paper)}</style>
  <script src="https://www.google.com/recaptcha/api.js?hl=es" async defer></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Alta de Cliente</h1>
  <asp:ValidationSummary runat="server" ValidationGroup="c" ForeColor="Maroon" />

  <div class="auth">
    <div class="grid2">
      <div>
        <label for="txtCuil">CUIL/CUIT</label>
        <asp:TextBox ID="txtCuil" runat="server" CssClass="input" MaxLength="14" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCuil" ErrorMessage="Ingresá CUIL/CUIT" ValidationGroup="c" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtCuil"
          ValidationExpression="^\d{11}$|^\d{2}-\d{8}-\d$" ErrorMessage="Formato CUIL inválido" ValidationGroup="c" />
      </div>
      <div>
        <label for="txtEmail">Email</label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" MaxLength="120" />
         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
      ErrorMessage="Ingresá el email." ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail"
      ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
      ErrorMessage="Email inválido." ValidationGroup="c" />
      </div>
      <div>
        <label for="txtNombre">Nombre</label>
        <asp:TextBox ID="txtNombre" runat="server" CssClass="input" MaxLength="60" />
         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNombre"
              ErrorMessage="Ingresá el nombre." ValidationGroup="c" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="txtNombre"
              ValidationExpression="^[A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- ']{2,60}$"
              ErrorMessage="Nombre inválido (solo letras/espacios, 2–60)."
              ValidationGroup="c" />
      </div>
      <div>
        <label for="txtApellido">Apellido</label>
        <asp:TextBox ID="txtApellido" runat="server" CssClass="input" MaxLength="60" />
           <asp:RequiredFieldValidator runat="server" ControlToValidate="txtApellido"
      ErrorMessage="Ingresá el apellido." ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtApellido"
      ValidationExpression="^[A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- ']{2,60}$"
      ErrorMessage="Apellido inválido (solo letras/espacios, 2–60)."
      ValidationGroup="c" />
      </div>
      <div>
        <label for="txtAlias">Negocio / Alias</label>
        <asp:TextBox ID="txtAlias" runat="server" CssClass="input" MaxLength="120" />
           <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAlias"
      ErrorMessage="Ingresá el alias del negocio." ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtAlias"
      ValidationExpression="^[0-9A-Za-zÁÉÍÓÚáéíóúÀ-ÖØ-öø-ÿÑñ\.\- '()&/]{2,120}$"
      ErrorMessage="Alias inválido (2–120, letras/nros/.,-/()&/)."
      ValidationGroup="c" />
      </div>
      <div>
        <label for="txtTel">Teléfono</label>
        <asp:TextBox ID="txtTel" runat="server" CssClass="input" MaxLength="30" />
          <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTel"
      ErrorMessage="Ingresá el teléfono." ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtTel"
      ValidationExpression="^[0-9()+\-.\s]{6,25}$"
      ErrorMessage="Teléfono inválido (6–25, dígitos + () - . espacios)."
      ValidationGroup="c" />
      </div>
        <div class="form-row">
          <label>Tipo de cliente</label>
          <asp:RadioButtonList ID="rblTipo" runat="server" RepeatDirection="Horizontal"
                               CssClass="input" OnSelectedIndexChanged="rblTipo_SelectedIndexChanged" AutoPostBack="false">
            <asp:ListItem Text="Autor" Value="AUT" Selected="True" />
            <asp:ListItem Text="Librería" Value="LIB" />
          </asp:RadioButtonList>
              <asp:RequiredFieldValidator runat="server" ControlToValidate="rblTipo"
      ErrorMessage="Elegí el tipo de cliente." ValidationGroup="c" />
        </div>

<div class="form-row" id="rowUbicacion" runat="server">
  <label for="txtUbicacion">Ubicación</label>
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
        <label for="txtDomFac">Domicilio fiscal</label>
        <asp:TextBox ID="txtDomFac" runat="server" CssClass="input" MaxLength="200" />
           <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDomFac"
      ErrorMessage="Ingresá el domicilio fiscal." ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtDomFac"
      ValidationExpression="^[^\r\n]{4,200}$"
      ErrorMessage="Domicilio inválido (4–200 caracteres, sin saltos de línea)."
      ValidationGroup="c" />
      </div>
      <div>
        <label for="txtEmailFac">Email facturación</label>
        <asp:TextBox ID="txtEmailFac" runat="server" CssClass="input" MaxLength="120" />
           <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmailFac"
      ErrorMessage="Ingresá el email de facturación." ValidationGroup="c" />
  <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmailFac"
      ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
      ErrorMessage="Email de facturación inválido." ValidationGroup="c" />
      </div>
    </div>

    <div class="g-recaptcha" data-sitekey="<%= System.Configuration.ConfigurationManager.AppSettings["RecaptchaSiteKeyLocal"] %>"></div>

    <div>
      <asp:Button ID="btnCrear" runat="server" Text="Crear cliente" CssClass="btn solid" OnClick="btnCrear_Click" ValidationGroup="c" />
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

