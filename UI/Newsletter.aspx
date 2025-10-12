<%@ Page Title="Newsletter" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Newsletter.aspx.cs" Inherits="Newsletter" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    /* Paleta del sitio */
    :root{
      --paper:#fff; --stroke:#e8dacd; --ink:#3b2f2a; --ink-soft:#7a6b5e;
      --brand:#a47148; --brand-dark:#6b4226;
    }

    .hero{background:#f5f0ea;border:1px solid var(--stroke);border-radius:14px;padding:22px;margin:16px 0}
    .hero h1{font-family:Georgia,serif;color:var(--brand-dark);margin:0 0 6px}

    .news-list{display:grid;gap:16px;max-width:980px}
    .news-item{
      display:grid; grid-template-columns:120px 1fr; gap:14px;
      background:var(--paper); border:1px solid var(--stroke); border-radius:16px; padding:14px;
      box-shadow:0 8px 18px rgba(0,0,0,.06)
    }
    .news-thumb{width:120px;height:90px;object-fit:cover;border-radius:10px;background:#fbf8f4}
    .news-title{margin:0;color:var(--brand-dark);font-family:Georgia,serif}
    .news-short{color:var(--ink-soft);margin:6px 0 10px}
    .btn{display:inline-block;padding:8px 12px;border-radius:10px;border:1px solid var(--brand);background:var(--brand);color:#fff;text-decoration:none}
    .btn.ghost{background:transparent;color:var(--brand)}
    .meta{color:var(--ink-soft);font-size:.9rem}

    /* Modal */
    .modal{position:fixed;inset:0;background:rgba(0,0,0,.45);display:none;align-items:center;justify-content:center;z-index:50}
    .modal.show{display:flex}
    .modal-card{background:#fff;max-width:840px;width:92%;border-radius:16px;overflow:auto;max-height:85vh;border:1px solid var(--stroke);box-shadow:0 18px 40px rgba(0,0,0,.25)}
    .modal-head{display:flex;justify-content:space-between;align-items:center;padding:14px 16px;border-bottom:1px solid var(--stroke)}
    .modal-head h3{margin:0;color:var(--brand-dark);font-family:Georgia,serif}
    .modal-body{padding:16px;display:grid;gap:12px}
    .modal-img{width:100%;height:300px;object-fit:cover;border-radius:10px;background:#fbf8f4}
    .close{border:0;background:transparent;font-size:22px;cursor:pointer;color:var(--ink-soft)}
    /* ancho/alto del modal */
.nl-modal .modal-dialog{ max-width: 860px; }
.nl-modal .modal-content{
  border-radius:16px;
  background: var(--paper);   /* fondo sólido */
  color: var(--ink);          /* texto legible */
  box-shadow: 0 18px 44px rgba(0,0,0,.2);
}
.nl-modal .modal-header,
.nl-modal .modal-body,
.nl-modal .modal-footer{
  background: var(--paper);
}

/* cuerpo con scroll vertical, sin scroll horizontal */
.nl-modal .modal-body{
  max-height: 70vh;
  overflow-y: auto;
  overflow-x: hidden;
  white-space: normal;
  word-break: break-word;
  overflow-wrap: anywhere;
  line-height: 1.55;
  padding: 16px 18px;
}

/* imagen dentro del modal */
.nl-modal .modal-body img{
  display:block;
  margin:0 auto 12px;
  max-width:100%;
  height:auto;
  max-height:45vh;        /* límite vertical */
  object-fit:contain;
}

/* backdrop un poco más oscuro para contraste */
.modal-backdrop.show{ opacity:.55; }

    @media (max-width:700px){ .news-item{grid-template-columns:1fr} .news-thumb{width:100%;height:180px} }
  </style>
  <script>
    window.addEventListener('DOMContentLoaded', function(){
      var modal = document.getElementById('newsModal');
      var mTitle = document.getElementById('mTitle');
      var mImg   = document.getElementById('mImg');
      var mBody  = document.getElementById('mBody');
      document.querySelectorAll('.js-more').forEach(function(btn){
        btn.addEventListener('click', function(){
          var host = btn.closest('.news-item');
          mTitle.textContent = host.getAttribute('data-title') || '';
          var img = host.getAttribute('data-img') || '';
          mImg.src = img || '/Content/blank-cover.png';
          // FullDescription llega como texto; preservo saltos de línea
          var full = host.getAttribute('data-full') || '';
          mBody.innerHTML = full.replace(/\r?\n/g,'<br/>');
          modal.classList.add('show');
        });
      });
      document.getElementById('mClose').addEventListener('click', function(){ modal.classList.remove('show'); });
      modal.addEventListener('click', function(e){ if(e.target === modal) modal.classList.remove('show'); });
    });
  </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="hero">
    <h1>Newsletter</h1>
    <div class="meta">Novedades, lanzamientos y artículos de la comunidad.</div>
  </div>
   <div class="hero nl-sub" style="margin-top:8px">
  <!-- Estado: NO suscripto -->
  <asp:Panel ID="pnlForm" runat="server" CssClass="nl-wrap">
    <h2>Suscribirme al newsletter</h2>

    <div>
      <label for="<%= txtEmail.ClientID %>">Email</label><br/>
      <asp:TextBox ID="txtEmail" runat="server" CssClass="input" />
      <asp:RequiredFieldValidator ID="reqEmail" runat="server"
          ControlToValidate="txtEmail" Display="Dynamic"
          ErrorMessage="El email es obligatorio." />
      <asp:RegularExpressionValidator ID="valEmail" runat="server"
          ControlToValidate="txtEmail" Display="Dynamic"
          ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
          ErrorMessage="Email inválido." />
    </div>

    <fieldset>
      <legend style="padding:0 6px;color:var(--ink-soft)">Categorías</legend>
      <label><asp:CheckBox ID="chkReco"  runat="server" /> Libros recomendados</label><br/>
      <label><asp:CheckBox ID="chkLaunch" runat="server" /> Nuevos lanzamientos</label><br/>
      <label><asp:CheckBox ID="chkEvtPres" runat="server" /> Eventos presenciales</label><br/>
      <label><asp:CheckBox ID="chkEvtVirt" runat="server" /> Eventos virtuales</label><br/>
      <asp:CustomValidator ID="valCats" runat="server" Display="Dynamic"
          OnServerValidate="valCats_ServerValidate"
          ErrorMessage="Elegí al menos una categoría." />
    </fieldset>

    <div class="nl-actions">
      <asp:Button ID="btnSubscribe" runat="server" CssClass="btn"
                  Text="Suscribirme" OnClick="btnSubscribe_Click" />
      <asp:Label ID="lblResult" runat="server" />
    </div>

    <asp:ValidationSummary ID="vs" runat="server" CssClass="meta" />
  </asp:Panel>

  <!-- Estado: YA suscripto -->
  <asp:Panel ID="pnlOk" runat="server" Visible="false" CssClass="nl-wrap">
    <h2>Suscripción</h2>
    <div class="meta">✅ Ya estás suscripto al newsletter.</div>
    <div class="nl-actions">
      <asp:Button ID="btnUnsub" runat="server" CssClass="btn ghost"
                  Text="Cancelar suscripción" OnClick="btnUnsub_Click"
                  OnClientClick="return confirm('¿Cancelar tu suscripción?');" />
      <asp:Label ID="lblUnsub" runat="server" />
    </div>
  </asp:Panel>
</div>



  <asp:Repeater ID="rptNews" runat="server">
    <HeaderTemplate><div class="news-list"></HeaderTemplate>
    <ItemTemplate>
      <div class="news-item"
           data-title='<%# System.Web.HttpUtility.HtmlAttributeEncode(Eval("Title") as string ?? "") %>'
           data-img='<%# System.Web.HttpUtility.HtmlAttributeEncode(Eval("ImageUrl") as string ?? "") %>'
           data-full='<%# System.Web.HttpUtility.HtmlAttributeEncode(Eval("FullDescription") as string ?? "") %>'>
        <img class="news-thumb" alt=""
             src='<%# string.IsNullOrEmpty(Eval("ImageUrl") as string) ? "/Content/blank-cover.png" : Eval("ImageUrl") %>' />
        <div>
          <h3 class="news-title"><%# Eval("Title") %></h3>
          <div class="meta"><%# string.Format("{0:yyyy-MM-dd HH:mm} UTC", Eval("CreatedUtc")) %></div>
          <div class="news-short"><%# Eval("ShortDescription") %></div>
          <button type="button" class="btn js-more">Leer más</button>
        </div>
      </div>
    </ItemTemplate>
    <FooterTemplate></div></FooterTemplate>
  </asp:Repeater>

  <div class="meta" style="margin-top:10px">
    <asp:HyperLink ID="lnkPrev" runat="server" CssClass="btn ghost" Text="⟵ Anteriores" />
    <asp:HyperLink ID="lnkNext" runat="server" CssClass="btn" Text="Siguientes ⟶" />
    &nbsp;<asp:Literal ID="litTotal" runat="server" />
  </div>

  <!-- MODAL -->
  <div class="modal fade nl-modal" id="newsModal" tabindex="-1" role="dialog">
    <div class="modal-dialog" role="document">
    <div class="modal-content">
        <h3 id="mTitle"></h3>
        <button id="mClose" class="close" aria-label="Cerrar">✕</button>
      </div>
      <div class="modal-body">
        <img id="mImg" class="modal-img" alt="">
        <div id="mBody" class="meta" style="color:var(--ink)"></div>
      </div>
    </div>
  </div>
</asp:Content>
