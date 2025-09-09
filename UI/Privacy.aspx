<%@ Page Title="Privacidad" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Privacy.aspx.cs" Inherits="Privacy" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server"><%: GetLocalResourceObject("Privacy_Title") %></asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: GetLocalResourceObject("Privacy_H1") %></h1>
  <p><%: GetLocalResourceObject("Privacy_Intro") %></p>

  <h2><%: GetLocalResourceObject("Privacy_Data_Title") %></h2>
  <ul>
    <li><%: GetLocalResourceObject("Privacy_Data_1") %></li>
    <li><%: GetLocalResourceObject("Privacy_Data_2") %></li>
    <li><%: GetLocalResourceObject("Privacy_Data_3") %></li>
  </ul>

  <h2><%: GetLocalResourceObject("Privacy_Use_Title") %></h2>
  <ul>
    <li><%: GetLocalResourceObject("Privacy_Use_1") %></li>
    <li><%: GetLocalResourceObject("Privacy_Use_2") %></li>
  </ul>

  <h2><%: GetLocalResourceObject("Privacy_Legal_Title") %></h2>
  <p>Consentimiento, ejecución de contrato y legítimo interés. Compartimos solo con procesadores necesarios
     (pagos, analytics) bajo acuerdos de tratamiento.</p>

  <h2><%: GetLocalResourceObject("Privacy_Security_Title") %></h2>
  <p>Conservamos por el tiempo necesario para prestar el servicio o por obligaciones legales.
     Aplicamos las medidas de seguridad indicadas en <a href="/Security.aspx">Políticas de seguridad</a>.</p>

  <h2><%: GetLocalResourceObject("Privacy_Rights_Title") %></h2>
  <p>Podés acceder, corregir, eliminar y exportar tus datos. Escribinos a
     <a href="mailto:privacy@literaryhub.example">privacy@literaryhub.example</a>.</p>

  <h2><%: GetLocalResourceObject("Privacy_Cookies_Title") %></h2>
  <p><%: GetLocalResourceObject("Privacy_Cookies_Body") %></p>

  <div class="photo-grid" style="margin-top:14px;">
    <div class="photo-slot"><%: GetLocalResourceObject("Privacy_Placeholder") %></div>
  </div>
</asp:Content>


