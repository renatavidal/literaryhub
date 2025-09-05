<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PaymentForm.ascx.cs" Inherits="Controls_PaymentForm" %>

<style>
  .pay{display:grid;gap:12px}
  .row{display:grid;gap:6px}
  .grid2{display:grid;grid-template-columns:1fr 1fr;gap:10px}
  .input{padding:10px;border:1px solid #efe7df;border-radius:10px;background:#fff}
  .btn{padding:10px 14px;border-radius:10px;border:1px solid #b89567;background:#c9a97a;color:#281c0f;cursor:pointer}
  .hint{color:#7a6b5e;font-size:.9rem}
</style>

<!-- Hidden para el libro / precio -->
<asp:HiddenField ID="hidGid" runat="server" />
<asp:HiddenField ID="hidTitle" runat="server" />
<asp:HiddenField ID="hidAuthors" runat="server" />
<asp:HiddenField ID="hidThumb" runat="server" />
<asp:HiddenField ID="hidIsbn13" runat="server" />
<asp:HiddenField ID="hidPub" runat="server" />
<asp:HiddenField ID="hidPrice" runat="server" />
<asp:HiddenField ID="hidCurrency" runat="server" />

<div class="pay">
  <!-- Titular -->
  <div class="row">
    <label for="<%=txtName.ClientID%>">Nombre del titular</label>
    <asp:TextBox ID="txtName" runat="server" CssClass="input" MaxLength="120"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName"
      ValidationGroup="pay" ErrorMessage="Ingresá el nombre del titular." Display="Dynamic" CssClass="hint" />
    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtName"
      ValidationGroup="pay" ValidationExpression="^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ'’\.\- ]{2,120}$"
      ErrorMessage="Nombre inválido." Display="Dynamic" CssClass="hint" />
  </div>

  <!-- Número -->
  <div class="row">
    <label for="<%=txtNumber.ClientID%>">Número de tarjeta</label>
    <asp:TextBox ID="txtNumber" runat="server" CssClass="input" MaxLength="19" placeholder="Sólo números"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumber"
      ValidationGroup="pay" ErrorMessage="Ingresá el número de tarjeta." Display="Dynamic" CssClass="hint" />
    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtNumber"
      ValidationGroup="pay" ValidationExpression="^\d{12,19}$"
      ErrorMessage="Ingresá 12 a 19 dígitos." Display="Dynamic" CssClass="hint" />
    <asp:CustomValidator ID="valLuhn" runat="server" ControlToValidate="txtNumber"
      ValidationGroup="pay" OnServerValidate="valLuhn_ServerValidate"
      ErrorMessage="Número de tarjeta inválido." Display="Dynamic" CssClass="hint" />
  </div>

  <!-- Vencimiento -->
  <div class="grid2">
    <div class="row">
      <label for="<%=ddlMonth.ClientID%>">Mes</label>
      <asp:DropDownList ID="ddlMonth" runat="server" CssClass="input" AppendDataBoundItems="true">
        <asp:ListItem Text="MM" Value="" />
      </asp:DropDownList>
      <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlMonth"
        InitialValue="" ValidationGroup="pay" ErrorMessage="Elegí el mes." Display="Dynamic" CssClass="hint" />
    </div>
    <div class="row">
      <label for="<%=ddlYear.ClientID%>">Año</label>
      <asp:DropDownList ID="ddlYear" runat="server" CssClass="input" AppendDataBoundItems="true">
        <asp:ListItem Text="AAAA" Value="" />
      </asp:DropDownList>
      <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlYear"
        InitialValue="" ValidationGroup="pay" ErrorMessage="Elegí el año." Display="Dynamic" CssClass="hint" />
    </div>
  </div>
  <asp:CustomValidator ID="valExp" runat="server"
    ValidationGroup="pay" OnServerValidate="valExp_ServerValidate"
    ErrorMessage="La tarjeta está vencida." Display="Dynamic" CssClass="hint" />

  <!-- CVV -->
  <div class="row">
    <label for="<%=txtCvv.ClientID%>">CVV (no se guarda)</label>
    <asp:TextBox ID="txtCvv" runat="server" CssClass="input" MaxLength="4" TextMode="Password"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCvv"
      ValidationGroup="pay" ErrorMessage="Ingresá el CVV." Display="Dynamic" CssClass="hint" />
    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtCvv"
      ValidationGroup="pay" ValidationExpression="^\d{3,4}$"
      ErrorMessage="CVV inválido." Display="Dynamic" CssClass="hint" />
    <asp:CustomValidator ID="valCvv" runat="server" ControlToValidate="txtCvv"
      ValidationGroup="pay" OnServerValidate="valCvv_ServerValidate"
      ErrorMessage="CVV inválido para la marca." Display="Dynamic" CssClass="hint" />
  </div>

  <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="pay" CssClass="hint" />

  <asp:Button ID="btnPay" runat="server" Text="Pagar"
    CssClass="btn" CausesValidation="true" ValidationGroup="pay" OnClick="btnPay_Click" />
  <asp:Literal ID="litStatus" runat="server" />
</div>
