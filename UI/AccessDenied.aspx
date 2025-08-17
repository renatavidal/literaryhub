<%@ Page Title="Acceso denegado" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="AccessDenied.aspx.cs" Inherits="AccessDenied" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .denied-card{
      background: var(--paper);
      border: 1px solid var(--stroke);
      border-radius: 16px;
      padding: 28px;
      max-width: 720px;
      margin: 16px 0;
    }
    .denied-actions{ display:flex; flex-wrap:wrap; gap:10px; margin-top:12px }
    .muted{ color: var(--ink-soft) }
    .lock{
      width: 54px; height: 54px; border-radius: 12px;
      display:inline-grid; place-items:center;
      background: var(--chip); border:1px solid var(--stroke);
      margin-bottom: 10px;
      font-size: 28px;
    }
  </style>
</asp:Content>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">
  Acceso denegado
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="denied-card">
    <div class="lock" aria-hidden="true">🔒</div>
    <h1 style="margin:0 0 6px 0;">Acceso denegado (403)</h1>
    <p class="muted" style="margin:0 0 12px 0;">
      No tenés permisos para acceder a esta sección.
      <asp:Literal ID="litUser" runat="server" />
    </p>

    <div class="denied-actions">
      <!-- Se muestra solo si NO está autenticado -->
      <asp:HyperLink ID="lnkLogin" runat="server" CssClass="btn solid" Text="Iniciar sesión" />

      <asp:HyperLink ID="lnkHome"  runat="server" CssClass="btn ghost" Text="Ir al inicio" NavigateUrl="/Landing.aspx" />
      <a href="javascript:history.back()" class="btn ghost">Volver</a>
      <a runat="server" href="/Contact.aspx" class="btn ghost">Contáctenos</a>
    </div>

    <p class="muted" style="margin-top:12px;">
      Si necesitás acceso, escribinos o contactá a un administrador.
    </p>
  </div>
</asp:Content>
