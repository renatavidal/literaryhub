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
    <label for="<%=txtName.ClientID%>"><%: GetLocalResourceObject("Pay_Label_Name") %></label>
    <asp:TextBox ID="txtName" runat="server" CssClass="input" MaxLength="120"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName"
      ValidationGroup="pay" ErrorMessage="<%$ Resources: Pay_Name_Required %>" Display="Dynamic" CssClass="hint" />
    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtName"
      ValidationGroup="pay" ValidationExpression="^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ'’\.\- ]{2,120}$"
      ErrorMessage="<%$ Resources: Pay_Name_Invalid %>" Display="Dynamic" CssClass="hint" />
  </div>

  <!-- Número -->
  <div class="row">
    <label for="<%=txtNumber.ClientID%>"><%: GetLocalResourceObject("Pay_Label_Number") %></label>
    <asp:TextBox ID="txtNumber" runat="server" CssClass="input" MaxLength="19" ></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumber"
      ValidationGroup="pay" ErrorMessage="<%$ Resources: Pay_Number_Required %>" Display="Dynamic" CssClass="hint" />
    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtNumber"
      ValidationGroup="pay" ValidationExpression="^\d{12,19}$"
      ErrorMessage="<%$ Resources: Pay_Number_InvalidDigits %>" Display="Dynamic" CssClass="hint" />
    <asp:CustomValidator ID="valLuhn" runat="server" ControlToValidate="txtNumber"
      ValidationGroup="pay" OnServerValidate="valLuhn_ServerValidate"
      ErrorMessage="<%$ Resources: Pay_Number_InvalidLuhn %>" Display="Dynamic" CssClass="hint" />
  </div>

  <!-- Vencimiento -->
  <div class="grid2">
    <div class="row">
      <label for="<%=ddlMonth.ClientID%>"><%: GetLocalResourceObject("Pay_Label_Month") %></label>
      <asp:DropDownList ID="ddlMonth" runat="server" CssClass="input" AppendDataBoundItems="true">
        <asp:ListItem Text="<%$ Resources: Pay_Month_MM %>" Value="" />
      </asp:DropDownList>
      <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlMonth"
        InitialValue="" ValidationGroup="pay" ErrorMessage="<%$ Resources: Pay_Month_Required %>" Display="Dynamic" CssClass="hint" />
    </div>
    <div class="row">
      <label for="<%=ddlYear.ClientID%>"><%: GetLocalResourceObject("Pay_Label_Year") %></label>
      <asp:DropDownList ID="ddlYear" runat="server" CssClass="input" AppendDataBoundItems="true">
        <asp:ListItem Text="<%$ Resources: Pay_Year_YYYY %>" Value="" />
      </asp:DropDownList>
      <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlYear"
        InitialValue="" ValidationGroup="pay" ErrorMessage="<%$ Resources: Pay_Year_Required %>" Display="Dynamic" CssClass="hint" />
    </div>
  </div>
  <asp:CustomValidator ID="valExp" runat="server"
    ValidationGroup="pay" OnServerValidate="valExp_ServerValidate"
    ErrorMessage="<%$ Resources: Pay_Expired %>" Display="Dynamic" CssClass="hint" />

  <!-- CVV -->
  <div class="row">
    <label for="<%=txtCvv.ClientID%>"><%: GetLocalResourceObject("Pay_Label_Cvv") %></label>
    <asp:TextBox ID="txtCvv" runat="server" CssClass="input" MaxLength="4" TextMode="Password"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCvv"
      ValidationGroup="pay" ErrorMessage="<%$ Resources: Pay_Cvv_Required %>" Display="Dynamic" CssClass="hint" />
    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtCvv"
      ValidationGroup="pay" ValidationExpression="^\d{3,4}$"
      ErrorMessage="<%$ Resources: Pay_Cvv_Invalid %>" Display="Dynamic" CssClass="hint" />
    <asp:CustomValidator ID="valCvv" runat="server" ControlToValidate="txtCvv"
      ValidationGroup="pay" OnServerValidate="valCvv_ServerValidate"
      ErrorMessage="<%$ Resources: Pay_Cvv_InvalidBrand %>" Display="Dynamic" CssClass="hint" />
  </div>

  <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="pay" CssClass="hint" />

<asp:Button ID="btnPay" runat="server"
    Text="<%$ Resources: Pay_PayButton %>"
    CssClass="btn"
    ValidationGroup="pay"
    CausesValidation="true"
    OnClick="btnPay_Click" />
  <asp:Literal ID="litStatus" runat="server" />
</div>

