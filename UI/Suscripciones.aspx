<%@ Page Title="Suscripciones" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Suscripciones.aspx.cs" Inherits="Suscripciones" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    /* Cards con tu paleta: cremas/marrones */
    .plans { display:grid; grid-template-columns:repeat(3,1fr); gap:18px; max-width:960px }
    @media(max-width:980px){ .plans{grid-template-columns:1fr} }
    .plan {
      overflow-x: scroll;
      background: var(--paper);
      border: 1px solid var(--stroke);
      border-radius: 18px;
      padding: 22px;
      box-shadow: 0 10px 22px rgba(0,0,0,.06);
      display:flex; flex-direction:column; gap:14px;
    }
    .plan.popular {
      background: linear-gradient(180deg, #f6efe6, var(--paper));
      border-color: #e3d6c7;
      transform: translateY(-6px);
    }
    .price { font-size:2.2rem; font-weight:800; color:var(--brand); }
    .per { color: var(--ink-soft); font-size:.95rem }
    .badge { background: var(--chip); color: var(--brand); padding:4px 10px; border-radius:999px; font-weight:700; font-size:.85rem }
    .features { list-style:none; padding:0; margin:0; display:grid; gap:8px }
    .features li { display:flex; align-items:center; gap:10px; color:var(--ink) }
    .ok { width:22px; height:22px; display:inline-grid; place-items:center; border-radius:50%; background:#dcd0c4 }
    .btn-choose {
      margin-top:auto; padding:10px 14px; border-radius:12px; border:1px solid var(--brand);
      background: var(--brand); color:#fff; text-decoration:none; text-align:center; font-weight:700;
    }
    .btn-choose:hover { background: var(--brand-dark) }
    .muted { color:var(--ink-soft) }
  </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Planes de suscripción</h1>
  <p class="muted">Elegí el plan que mejor se adapte a vos. Los precios están expresados en USD.</p>

  <asp:Repeater ID="rptPlanes" runat="server">
    <HeaderTemplate><div class="plans"></HeaderTemplate>
    <ItemTemplate>
      <div class='plan <%# ((bool)Eval("EsDestacado")) ? "popular" : "" %>'>
        <div class="badge"><%# Eval("Descripcion") %></div>

        <div>
          <div class="price">$<%# Eval("PrecioUSD","{0:0.##}") %></div>
          <div class="per">/mes</div>
        </div>

        <ul class="features">
          <li><span class="ok">✓</span> Roles incluidos: <strong><%# Eval("Roles") %></strong></li>
          <li><span class="ok">✓</span> Acceso a la comunidad</li>
          <li><span class="ok">✓</span> Panel de usuario</li>
          <li><span class="ok">✓</span> Soporte básico</li>
        </ul>

        <a class="btn-choose" href='<%# "/Checkout.aspx?plan=" + Eval("Codigo") %>'>
          Elegir plan
        </a>
      </div>
    </ItemTemplate>
    <FooterTemplate></div></FooterTemplate>
  </asp:Repeater>
</asp:Content>
