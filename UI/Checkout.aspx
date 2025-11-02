<%@ Page Title="Checkout" Language="C#" MasterPageFile="~/site.master" AutoEventWireup="true" CodeFile="Checkout.aspx.cs" Inherits="Checkout"%>
<%@ Register Src="~/Controls/PaymentForm.ascx" TagPrefix="lh" TagName="PaymentForm" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .checkout{display:grid;gap:18px;max-width:960px}
    .card{background:var(--paper);border:1px solid var(--stroke);border-radius:16px;padding:18px}
    .grid2{display:grid;grid-template-columns:1fr 1fr;gap:12px}
    .row{display:grid;gap:6px}
    .input{padding:10px;border:1px solid #efe7df;border-radius:10px;background:#fff}
    .btn{padding:10px 14px;border-radius:10px;border:1px solid #b89567;background:#c9a97a;color:#281c0f;cursor:pointer}
    .muted{color:#7a6b5e}
  </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="checkout">

    <h1>Checkout</h1>
    <p class="muted">Completá el pago de tu suscripción. Podés combinar Tarjeta, Nota de Crédito y Cuenta Corriente.</p>

    <!-- Resumen del plan -->
    <div class="card">
      <div class="grid2">
        <div class="row">
          <label>Plan</label>
          <asp:TextBox ID="txtPlan" runat="server" CssClass="input" ReadOnly="true"></asp:TextBox>
        </div>
        <div class="row">
          <label>Total (USD)</label>
          <asp:TextBox ID="txtTotal" runat="server" CssClass="input" ReadOnly="true"></asp:TextBox>
             <asp:HiddenField ID="hidPlanTotal" runat="server" />
        </div>
      </div>
    </div>

    <!-- Métodos de pago -->
    <div class="card">
      <h3 style="margin-top:0">Pago con Tarjeta</h3>
      <asp:CheckBox ID="chkCard" runat="server" Text="Usar tarjeta" Checked="true" />
      <div class="grid2" style="margin-top:8px">
      <!-- Tarjeta -->
        <div class="row">
          <label>Monto con tarjeta (USD)</label>
          <asp:TextBox ID="txtCardAmount" runat="server" CssClass="input" MaxLength="10" />
          <asp:RegularExpressionValidator ID="rxCard" runat="server"
            ControlToValidate="txtCardAmount" ValidationGroup="chk"
            ValidationExpression="^\d{1,7}(\.\d{1,2})?$"
            ErrorMessage="Monto inválido (hasta 2 decimales)." CssClass="hint" Display="Dynamic" />
            <asp:CustomValidator ID="valCardAmt" runat="server"
  ControlToValidate="txtCardAmount" ValidationGroup="chk"
  OnServerValidate="valCardAmt_ServerValidate"
  Display="Dynamic" CssClass="hint"
  ErrorMessage="Monto con tarjeta debe ser > 0." />
        </div>
        <div></div>
      </div>
      <!-- Tu control reutilizable -->
      <lh:PaymentForm ID="paymentForm" runat="server"  Standalone="false"  />
    </div>

    <div class="card">
      <h3 style="margin-top:0">Aplicar Nota de Crédito</h3>
      <asp:CheckBox ID="chkNC" runat="server" Text="Usar nota de crédito" />
      <div class="grid2" style="margin-top:8px">
       <div class="grid2" style="margin-top:8px">
  <div class="row">
    <label>Id de Nota</label>
    <asp:TextBox ID="txtNcId" runat="server" CssClass="input" MaxLength="100" />
   
  </div>
  <div class="row">
    <label>Monto a usar (USD)</label>
    <asp:TextBox ID="txtNcAmount" runat="server" CssClass="input" MaxLength="10" />
    <asp:RegularExpressionValidator ID="rxNc" runat="server"
      ControlToValidate="txtNcAmount" ValidationGroup="chk"
      ValidationExpression="^\d{1,7}(\.\d{1,2})?$"
      ErrorMessage="Monto inválido (hasta 2 decimales)." CssClass="hint" Display="Dynamic" />
    <asp:CustomValidator ID="valNcFunds" runat="server" ControlToValidate="txtNcAmount"
      ValidationGroup="chk" OnServerValidate="valNcFunds_ServerValidate"
      ErrorMessage="La Nota de Crédito no tiene saldo suficiente." CssClass="hint" Display="Dynamic" />
      <asp:CustomValidator ID="valNcIdRequired" runat="server"
  ControlToValidate="txtNcId" ValidationGroup="chk"
  OnServerValidate="valNcIdRequired_ServerValidate"
  Display="Dynamic" CssClass="hint"
  ErrorMessage="Indicá el Id de la NC." />
      <asp:CustomValidator ID="valNcAmt" runat="server"
  ControlToValidate="txtNcAmount" ValidationGroup="chk"
  OnServerValidate="valNcAmt_ServerValidate"
  Display="Dynamic" CssClass="hint"
  ErrorMessage="Monto de NC debe ser > 0." />
  </div>
</div>
      </div>
      <small class="muted">Ingresá la NC disponible y el monto a aplicar.</small>
    </div>

    <div class="card">
      <h3 style="margin-top:0">Usar Cuenta Corriente</h3>
      <asp:CheckBox ID="chkAccount" runat="server" Text="Descontar de mi saldo" />
    <div class="row" style="margin-top:8px">
  <label>Monto a descontar (USD)</label>
  <asp:TextBox ID="txtAccountAmount" runat="server" CssClass="input" MaxLength="10" />
  <asp:RegularExpressionValidator ID="rxAcc" runat="server"
    ControlToValidate="txtAccountAmount" ValidationGroup="chk"
    ValidationExpression="^\d{1,7}(\.\d{1,2})?$"
    ErrorMessage="Monto inválido (hasta 2 decimales)." CssClass="hint" Display="Dynamic" />
  <asp:CustomValidator ID="valAccFunds" runat="server" ControlToValidate="txtAccountAmount"
    ValidationGroup="chk" OnServerValidate="valAccFunds_ServerValidate"
    ErrorMessage="Saldo de cuenta corriente insuficiente." CssClass="hint" Display="Dynamic" />
        <asp:CustomValidator ID="valAccAmt" runat="server"
  ControlToValidate="txtAccountAmount" ValidationGroup="chk"
  OnServerValidate="valAccAmt_ServerValidate"
  Display="Dynamic" CssClass="hint"
  ErrorMessage="Monto de cuenta corriente debe ser > 0." />
</div>
    </div>
      <asp:CustomValidator ID="valTotals" runat="server" ValidationGroup="chk"
  OnServerValidate="valTotals_ServerValidate"
  ErrorMessage="La suma de pagos debe igualar el total del plan."
  CssClass="hint" Display="Dynamic" />
      <asp:CustomValidator ID="valAnyMethod" runat="server"
  ValidationGroup="chk" OnServerValidate="valAnyMethod_ServerValidate"
  Display="Dynamic" CssClass="hint"
  ErrorMessage="Elegí al menos un método de pago (Tarjeta, Nota de crédito o Cuenta Corriente)." />


<asp:ValidationSummary ID="valSummary" runat="server" ValidationGroup="chk" CssClass="hint" />

    <div>
      <asp:Button ID="btnPagar" runat="server" Text="Pagar" CssClass="btn"
            OnClick="btnPagar_Click" />
      <asp:Literal ID="litMsg" runat="server" />
        <asp:Label ID="outputmessage" runat="server" />
    </div>

  </div>
</asp:Content>
