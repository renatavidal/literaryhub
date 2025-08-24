<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Forgot.aspx.cs" Inherits="Forgot" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Recuperar contraseña</title>
  <style>
    body{font-family:Arial;background:#faf7f4;margin:0}
    .box{max-width:420px;margin:40px auto;background:#fff;padding:24px;border-radius:12px;box-shadow:0 2px 6px rgba(0,0,0,.1)}
    h2{color:#6b4226;margin:0 0 16px}
    .row{margin-bottom:12px}
    input[type=text],input[type=email],input[type=password]{width:100%;padding:10px;border:1px solid #ccc;border-radius:6px}
    .btn{background:#a47148;color:#fff;border:none;padding:10px 16px;border-radius:6px;font-weight:bold;cursor:pointer}
    .msg{margin-top:10px;font-size:13px}
    .ok{color:green}.err{color:#b00020}
  </style>
</head>
<body>
  <form id="form1" runat="server">
    <div class="box">
      <h2>Recuperar contraseña</h2>

      <!-- Paso 1: pedir email -->
      <asp:Panel ID="pnlStep1" runat="server">
        <div class="row">
          <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="Tu email" />
        </div>
        <asp:Button ID="btnSendCode" runat="server" Text="Enviar código" CssClass="btn" OnClick="btnSendCode_Click" />
        <asp:Label ID="lblMsg1" runat="server" CssClass="msg"></asp:Label>
      </asp:Panel>

      <!-- Paso 2: ingresar código + nueva contraseña -->
      <asp:Panel ID="pnlStep2" runat="server" Visible="false">
       
        <div class="row">
          <asp:TextBox ID="txtNewPass" runat="server" TextMode="Password" placeholder="Nueva contraseña" />
             <asp:RequiredFieldValidator ID="reqPassword" runat="server"
           ControlToValidate="txtNewPass" ErrorMessage="Ingresá tu contraseña."
           Display="Dynamic" SetFocusOnError="true" ValidationGroup="login" />
            <asp:RegularExpressionValidator ID="revPassword" runat="server"
              ControlToValidate="txtNewPass" ValidationExpression="^.{8,128}$"
              ErrorMessage="La contraseña debe tener entre 8 y 128 caracteres."
              Display="Dynamic" ValidationGroup="login" />
        </div>
        <div class="row">
          <asp:TextBox ID="txtNewPass2" runat="server" TextMode="Password" placeholder="Confirmar contraseña" />
              <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
            ControlToValidate="txtNewPass2" ErrorMessage="Ingresá tu contraseña."
            Display="Dynamic" SetFocusOnError="true" ValidationGroup="login" />
             <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server"
               ControlToValidate="txtNewPass2" ValidationExpression="^.{8,128}$"
               ErrorMessage="La contraseña debe tener entre 8 y 128 caracteres."
               Display="Dynamic" ValidationGroup="login" />
        </div>
        <asp:Button ID="btnReset" runat="server" Text="Cambiar contraseña" CssClass="btn" OnClick="btnReset_Click" />
        <asp:Label ID="lblMsg2" runat="server" CssClass="msg"></asp:Label>
      </asp:Panel>

      <!-- guardamos userId y un id del código en ViewState/Hidden -->
      <asp:HiddenField ID="hidUserId" runat="server" />
      <asp:HiddenField ID="hidCodeId" runat="server" />
    </div>
  </form>
</body>
</html>
