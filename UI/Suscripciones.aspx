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
    .compare-grid { overflow-x:auto; }
    .compare-cols { display:grid; grid-auto-flow:column; grid-auto-columns: minmax(240px, 1fr); gap:16px; }
    .compare-col {
      background: var(--paper);
      border:1px solid var(--stroke);
      border-radius:16px;
      padding:16px;
      box-shadow:0 8px 18px rgba(0,0,0,.06);
    }
    .compare-col.popular{
      background: linear-gradient(180deg, #f6efe6, var(--paper));
      border-color:#e3d6c7;
    }
    .compare-head{ display:grid; gap:6px; margin-bottom:12px }
    .compare-price{ font-size:1.6rem; font-weight:800; color:var(--brand) }
    .compare-features{ list-style:none; display:grid; gap:8px; padding:0; margin:0 }
    .badge.mini{ display:inline-block; padding:2px 8px; border-radius:999px; background:var(--chip); color:var(--brand); font-weight:700; font-size:.8rem }
    .btn-ghost{ border:1px solid var(--brand); padding:8px 12px; border-radius:10px; text-decoration:none; color:var(--brand) }
  </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: GetLocalResourceObject("Subs_Title") %></h1>
  <p class="muted"><%: GetLocalResourceObject("Subs_Intro") %></p>
    <!-- ======= BARRA DE COMPARACIÓN ======= -->
  <div class="compare-bar" style="margin:18px 0; padding:14px; border:1px solid var(--stroke); border-radius:12px; background:var(--paper); display:grid; gap:10px">
    <div style="display:grid; gap:10px">
      <strong><%: GetLocalResourceObject("Subs_Compare_Title") %></strong>
      <div class="selectors" style="display:grid; grid-template-columns:repeat(3, minmax(160px, 1fr)); gap:10px">
        <asp:DropDownList ID="ddlA" runat="server" CssClass="input"></asp:DropDownList>
        <asp:DropDownList ID="ddlB" runat="server" CssClass="input"></asp:DropDownList>
        <asp:DropDownList ID="ddlC" runat="server" CssClass="input"></asp:DropDownList>
      </div>
      <div>
        <asp:Button ID="btnCompare" runat="server" Text="<%$ Resources: Subs_Compare_Button %>" OnClick="btnCompare_Click"
                    CssClass="btn-choose" />
      </div>
    </div>
    <small class="muted" id="comparemessage"><%: GetLocalResourceObject("Subs_Compare_SelectMessage") %><%: GetLocalResourceObject("Subs_Compare_SelectMessage") %></small>
  </div>
      <asp:PlaceHolder ID="phCompare" runat="server" />
  <asp:Repeater ID="rptPlanes" runat="server">
    <HeaderTemplate><div class="plans"></HeaderTemplate>
    <ItemTemplate>
      <div class='plan <%# ((bool)Eval("EsDestacado")) ? "popular" : "" %>'>
        <div class="badge"><%# Eval("Descripcion") %></div>

        <div>
          <div class="price">$<%# Eval("PrecioUSD","{0:0.##}") %></div>
          <div class="per"><%: GetLocalResourceObject("Subs_PerMonth") %></div>
        </div>

        <ul class="features">
          <li><span class="ok">✓</span> <%: GetLocalResourceObject("Subs_Feature_RolesPrefix") %> <strong><%# Eval("Roles") %></strong></li>
          <li><span class="ok">✓</span> Acceso a la comunidad</li>
          <li><span class="ok">✓</span> Panel de usuario</li>
          <li><span class="ok">✓</span> Soporte básico</li>
        </ul>

        <a class="btn-choose" href='<%# "/Checkout.aspx?plan=" + Eval("Codigo") %>'><%: GetLocalResourceObject("Subs_ChoosePlan") %></a>
            <!-- Botón que precarga el selector A/B con este plan -->
          <asp:LinkButton runat="server" CommandName="prefill"
              CommandArgument='<%# Eval("Codigo") %>' CssClass="btn-ghost">Comparar este</asp:LinkButton>
      </div>
    </ItemTemplate>
    <FooterTemplate></div></FooterTemplate>
  </asp:Repeater>
    
</asp:Content>


