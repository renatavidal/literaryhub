<%@ Page Title="Políticas de seguridad" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Security.aspx.cs" Inherits="Security" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Políticas de seguridad</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Políticas de seguridad</h1>
  <p>Protegemos la confidencialidad, integridad y disponibilidad de los datos de nuestra comunidad.</p>

  <h2>Seguridad de la información</h2>
  <ul>
    <li>Cifrado TLS en tránsito; cifrado de datos sensibles en reposo.</li>
    <li>Controles de acceso por rol y autenticación reforzada.</li>
    <li>Registro de eventos y monitoreo para detección de anomalías.</li>
  </ul>

  <h2>Pagos</h2>
  <p>Transacciones procesadas por proveedores certificados; no almacenamos datos completos de tarjetas.</p>

  <h2>Infraestructura</h2>
  <ul>
    <li>Backups programados y pruebas de restauración.</li>
    <li>Parcheo periódico y revisión de dependencias.</li>
    <li>Separación de ambientes (dev, test, prod).</li>
  </ul>

  <h2>Reporte de vulnerabilidades</h2>
  <p>Si detectás una vulnerabilidad, escribinos a <a href="mailto:security@literaryhub.example">security@literaryhub.example</a>.</p>

  <div class="photo-grid" style="margin-top:14px;">
    <div class="photo-slot">Espacio para diagrama/infra</div>
  </div>
</asp:Content>
