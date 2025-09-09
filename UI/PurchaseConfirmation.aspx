<%@ Page Title="Compra confirmada" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="PurchaseConfirmation.aspx.cs" Inherits="PurchaseConfirmation" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <style>
    .conf{background:#fff;border:1px solid #efe7df;border-radius:12px;padding:18px;margin:16px 0;max-width:720px}
    .title{font-family:Georgia,serif;color:#3b2f2a;font-size:22px;margin:0 0 8px}
    .muted{color:#7a6b5e}
    .row{margin:6px 0}
    .btn{display:inline-block;margin-top:12px;padding:10px 14px;border-radius:10px;border:1px solid #b89567;background:#c9a97a;color:#281c0f;text-decoration:none}
  </style>

  <div class="conf">
    <h1 class="title"><%: GetLocalResourceObject("PC_Thanks") %></h1>
    <div class="row muted"><%: GetLocalResourceObject("PC_OrderNumber") %> <strong><asp:Literal ID="litId" runat="server"/></strong></div>

    <!-- Detalles si están disponibles -->
    <div id="boxDetails" runat="server" visible="false">
      <div class="row"><strong><%: GetLocalResourceObject("PC_Label_Title") %></strong> <asp:Literal ID="litTitle" runat="server"/></div>
      <div class="row"><strong><%: GetLocalResourceObject("PC_Label_Authors") %></strong> <asp:Literal ID="litAuthors" runat="server"/></div>
      <div class="row"><strong><%: GetLocalResourceObject("PC_Label_Amount") %></strong> <asp:Literal ID="litAmount" runat="server"/></div>
      <div class="row"><strong><%: GetLocalResourceObject("PC_Label_Status") %></strong> <asp:Literal ID="litStatus" runat="server"/></div>
      <div class="row muted"><asp:Literal ID="litWhen" runat="server"/></div>
    </div>

    <a href="/Home.aspx" class="btn"><%: GetLocalResourceObject("PC_BackHome") %></a>
  </div>
</asp:Content>

