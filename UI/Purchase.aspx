<%@ Page Title="Comprar libro" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Purchase.aspx.cs" Inherits="Purchase" %>
<%@ Register Src="~/Controls/PaymentForm.ascx" TagPrefix="lh" TagName="PaymentForm" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <style>
    .buy-wrap{display:grid;grid-template-columns:260px 1fr;gap:20px}
    .buy-book{background:#fff;border:1px solid #efe7df;border-radius:12px;padding:12px}
    .buy-book .cover{width:100%;border-radius:10px;border:1px solid #efe7df}
    .buy-book h2{font-family:Georgia,serif;color:#3b2f2a;margin:10px 0 6px;font-size:20px}
    .muted{color:#7a6b5e}
    @media(max-width:900px){.buy-wrap{grid-template-columns:1fr}}
  </style>

  <div class="buy-wrap">
    <!-- Columna izq: resumen del libro -->
    <div class="buy-book">
      <img id="imgCover" class="cover" src="/Content/blank-cover.png" alt="Portada" />
      <h2 id="hTitle"><%: GetLocalResourceObject("Purch_Loading") %></h2>
      <div class="muted" id="hAuthors"></div>
      <div class="muted" id="hInfo"></div>
      <div style="margin-top:10px;font-weight:bold" id="hPrice"></div>
    </div>

    <!-- Columna der: formulario de pago -->
    <lh:PaymentForm ID="PaymentForm1" runat="server" />
  </div>

  <script>
    // Carga datos del libro y setea hidden fields del control
    (function(){
      function s(el, v){ if(el) el.textContent = v || ""; }
      function id(x){ return document.getElementById(x); }

      var params = new URLSearchParams(location.search);
      var gid = params.get("gid");
      if(!gid){ return; }

      var api = "https://www.googleapis.com/books/v1/volumes/" + encodeURIComponent(gid);
      fetch(api).then(r => r.json()).then(v => {
        var info = v.volumeInfo || {};
        var sale = v.saleInfo || {};
        var img = (info.imageLinks && (info.imageLinks.thumbnail || info.imageLinks.smallThumbnail)) || "/Content/blank-cover.png";
        id("imgCover").src = img;
        s(id("hTitle"), info.title || "<%= GetLocalResourceObject("Purch_Untitled") %>");
        s(id("hAuthors"), (info.authors||[]).join(", "));
        s(id("hInfo"), (info.publishedDate||"") + (info.publisher? " · " + info.publisher : ""));

        // Precio (si existe en Google); fallback a 9.99 USD para demo
        var price = null, curr = "USD";
        if (sale && sale.listPrice && sale.listPrice.amount) {
          price = sale.listPrice.amount;
          curr  = sale.listPrice.currencyCode || "USD";
        } else {
          price = 9.99; curr = "USD";
        }
        s(id("hPrice"), (price ? (curr + " " + price.toFixed ? price.toFixed(2) : price) : "—"));

        // Setear hidden del control
          document.getElementById("<%= PaymentForm1.HidGidClientID %>").value   = gid;
          document.getElementById("<%= PaymentForm1.HidTitleClientID %>").value = info.title || "";
        document.getElementById("<%= PaymentForm1.HidAuthorsClientID %>").value = (info.authors||[]).join(", ");
        document.getElementById("<%= PaymentForm1.HidThumbClientID %>").value = img || "";
        document.getElementById("<%= PaymentForm1.HidIsbn13ClientID %>").value = ((info.industryIdentifiers||[]).filter(function(x){return x.type==="ISBN_13";}).map(function(x){return x.identifier;})[0]) || "";
        document.getElementById("<%= PaymentForm1.HidPubClientID %>").value = info.publishedDate || "";
        document.getElementById("<%= PaymentForm1.HidPriceClientID %>").value = price;
        document.getElementById("<%= PaymentForm1.HidCurrencyClientID %>").value = curr;
      }).catch(function(){ /* silencioso */ });
    })();
  </script>
</asp:Content>

