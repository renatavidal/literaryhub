<%@ Page Title="Verificación de email pendiente" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="VerifyEmailPending.aspx.cs" Inherits="VerifyEmailPending" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .verify-card{
      background: var(--paper);
      border: 1px solid var(--stroke);
      border-radius: 16px;
      padding: 28px;
      max-width: 720px;
      margin: 16px 0;
    }
    .muted{ color: var(--ink-soft) }
    .verify-actions{ display:flex; flex-wrap:wrap; gap:10px; margin-top:12px }
    .envelope{
      width: 54px; height: 54px; border-radius: 12px;
      display:inline-grid; place-items:center;
      background: var(--chip); border:1px solid var(--stroke);
      margin-bottom: 10px; font-size: 28px;
    }
    .hint{ font-size:.95rem; color: var(--ink-soft); margin-top:8px }
    .ok{ color: green } .err{ color: maroon }
  </style>
</asp:Content>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">
  Verificación de email pendiente
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="verify-card">
    <div class="envelope" aria-hidden="true">✉️</div>
    <h1 style="margin:0 0 6px 0;">Confirmá tu correo</h1>

    <p class="muted" style="margin:0 0 12px 0;">
      Te enviamos un enlace de verificación a
      <strong><asp:Literal ID="litMaskedEmail" runat="server" /></strong>.
      Seguí las instrucciones para activar tu cuenta.
    </p>

    <div class="verify-actions">
      <asp:Button ID="btnResend" runat="server" CssClass="btn solid" Text="Reenviar verificación" OnClick="btnResend_Click" />
      <asp:HyperLink ID="lnkLogout" runat="server" CssClass="btn ghost" Text="Cerrar sesión" NavigateUrl="/Logout.aspx" />
    </div>

    <div class="hint">
      Revisá la carpeta de spam y asegurate de que <code>literary.hub.contact@gmail.com</code> esté permitido.
    </div>

    <asp:Label ID="lblStatus" runat="server" />
  </div>
</asp:Content>
