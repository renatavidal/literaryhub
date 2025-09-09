<%@ Page Title="Políticas de seguridad" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Security.aspx.cs" Inherits="Security" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server"><%: GetLocalResourceObject("Security_Title") %></asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: GetLocalResourceObject("Security_H1") %></h1>
  <p><%: GetLocalResourceObject("Security_Intro") %></p>

  <h2><%: GetLocalResourceObject("Security_Sec_InfoSec") %></h2>
  <ul>
    <li><%: GetLocalResourceObject("Security_InfoSec_Item1") %></li>
    <li><%: GetLocalResourceObject("Security_InfoSec_Item2") %></li>
    <li><%: GetLocalResourceObject("Security_InfoSec_Item3") %></li>
  </ul>

  <h2><%: GetLocalResourceObject("Security_Sec_Payments") %></h2>
  <p><%: GetLocalResourceObject("Security_Payments_Body") %></p>

  <h2><%: GetLocalResourceObject("Security_Sec_Infrastructure") %></h2>
  <ul>
    <li><%: GetLocalResourceObject("Security_Infrastructure_Item1") %></li>
    <li><%: GetLocalResourceObject("Security_Infrastructure_Item2") %></li>
    <li><%: GetLocalResourceObject("Security_Infrastructure_Item3") %></li>
  </ul>

  <h2><%: GetLocalResourceObject("Security_Sec_VulnReport") %></h2>
  <p><%: GetLocalResourceObject("Security_Vuln_Body") %>vulnerabilidad, escribinos a <a href="mailto:security@literaryhub.example">security@literaryhub.example</a>.</p>

  <div class="photo-grid" style="margin-top:14px;">
    <div class="photo-slot">Espacio para diagrama/infra</div>
  </div>
</asp:Content>

