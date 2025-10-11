<%@ Page Title="Checkout OK" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="CheckoutOk.aspx.cs" Inherits="CheckoutOk" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <style>
    .cko { max-width: 720px; margin: 24px auto; padding: 18px; background:#fff; border:1px solid #e5e7eb; border-radius:12px }
    .stars { display:inline-flex; flex-direction: row-reverse; gap: 6px; }
    .stars input { position: absolute; left:-9999px; } /* hide radios */
    .stars label {
      font-size: 28px; line-height:1; cursor: pointer; user-select:none;
      filter: grayscale(1) opacity(.45);
      transition: transform .08s ease, filter .15s ease;
    }
    .stars label:hover, .stars label:hover ~ label { transform: scale(1.1); filter:none; }
    .stars input:checked ~ label { filter:none; }
    .meta { margin-top: 10px; color:#374151; }
    .row { display:flex; align-items:center; gap:12px; flex-wrap:wrap; }
    .pill { padding:4px 10px; border-radius:999px; background:#f3f4f6; font-size:12px; }
    .btn-clear {
      border:none; background:#f3f4f6; padding:6px 10px; border-radius:8px; cursor:pointer;
    }
    .ok { color:#059669; font-weight:600 }
    .err { color:#b91c1c; font-weight:600 }
  </style>

  <div class="cko">
    <h2>Thanks for your purchase 🎉</h2>
    <p>Please rate your product:</p>

    <asp:HiddenField ID="hfProductId" runat="server" />
    <asp:HiddenField ID="hfInitialRating" runat="server" Value="0" />

   <style>
  .btn-send{ border:none; background:#111827; color:#fff; padding:8px 14px; border-radius:8px; cursor:pointer }
</style>
...
<div class="row" aria-label="Rate from 0 to 5 stars">
  <div class="stars" id="starGroup">
    <input type="radio" id="star0" name="rating" value="0" />
    <input type="radio" id="star5" name="rating" value="5" /><label for="star5">★</label>
    <input type="radio" id="star4" name="rating" value="4" /><label for="star4">★</label>
    <input type="radio" id="star3" name="rating" value="3" /><label for="star3">★</label>
    <input type="radio" id="star2" name="rating" value="2" /><label for="star2">★</label>
    <input type="radio" id="star1" name="rating" value="1" /><label for="star1">★</label>
  </div>

  <button type="button" class="btn-clear" id="btnClear" title="Set 0 stars">Clear (0)</button>

<asp:Button ID="btnSend" runat="server" CssClass="btn-send"
    Text="Send" OnClick="btnSend_Click"
    OnClientClick="sendRating(); return false;" UseSubmitBehavior="false" />

  <!-- OnClientClick intenta AJAX; si JS no está, hace postback y usa btnSend_Click -->
  
  <span class="pill"><span id="youRated">You rated: 0</span> / 5</span>
</div>

    <div class="meta">
      Average: <strong id="avg">—</strong> (from <span id="cnt">—</span> ratings)
      <span id="msg" style="margin-left:10px;"></span>
    </div>
  </div>

  <script>
      (function () {
          var productId = parseInt(document.getElementById('<%= hfProductId.ClientID %>').value || '0', 10);
  var init = parseInt(document.getElementById('<%= hfInitialRating.ClientID %>').value || '0', 10);
          var you = document.getElementById('youRated');
          var avg = document.getElementById('avg');
          var cnt = document.getElementById('cnt');
          var msg = document.getElementById('msg');

          function select(val) {
              var el = document.getElementById('star' + val);
              if (el) el.checked = true;
              you.textContent = 'You rated: ' + val;
          }
          function show(ok, text) {
              msg.className = ok ? 'ok' : 'err';
              msg.textContent = text;
              if (text) setTimeout(function () { msg.textContent = ''; }, 2500);
          }
          function getSelected() {
              var sel = document.querySelector('input[name=rating]:checked');
              return sel ? parseInt(sel.value, 10) : 0;
          }
          function save(val) {
              if (!productId) { show(false, 'Missing product'); return false; }
              return fetch('CheckoutOk.aspx/SaveRating', {
                  method: 'POST', headers: { 'Content-Type': 'application/json; charset=utf-8' },
                  body: JSON.stringify({ productId: productId, rating: parseInt(val, 10) })
              })
                  .then(r => r.ok ? r.json() : Promise.reject())
                  .then(payload => {
                      var d = payload.d || payload;
                      if (d && d.ok) {
                          avg.textContent = d.avg.toFixed(2);
                          cnt.textContent = d.count;
                          show(true, 'Saved!');
                          return true;
                      } else {
                          show(false, (d && d.error) || 'Error'); return false;
                      }
                  })
                  .catch(() => { show(false, 'Network error'); return false; });
          }

          // Solo cambia la selección visual; NO guarda hasta apretar Send
          Array.prototype.forEach.call(document.querySelectorAll('input[name=rating]'), function (r) {
              r.addEventListener('change', function () { select(r.value); });
          });
          document.getElementById('btnClear').addEventListener('click', function () {
              select(0);
          });

          // Expuesto para OnClientClick del botón
          window.sendRating = function () {
              var val = getSelected();
              select(val);
              save(val);          
              return false;       
          };

          // Estado inicial
          select(init);
      })();
  </script>

</asp:Content>
