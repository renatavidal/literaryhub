<%@ Page Title="Soporte" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="SupportChat.aspx.cs" Inherits="SupportChat" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <style>
    .chat-wrap{max-width:720px;margin:16px auto;background:#fff;border:1px solid var(--stroke);border-radius:14px;display:flex;flex-direction:column;height:70vh}
    .chat-head{padding:12px 14px;border-bottom:1px solid var(--stroke);font-weight:700}
    .chat-body{flex:1;overflow:auto;padding:12px;background:#f6f3ef}
    .msg{max-width:70%;margin:8px 0;padding:10px 12px;border-radius:14px;line-height:1.3;white-space:pre-wrap}
    .mine{background:#3b2f2a;color:#fff; margin-left:auto; border-bottom-right-radius:4px}
    .theirs{background:#fff; border:1px solid #e5e7eb; color:#1f2937; margin-right:auto; border-bottom-left-radius:4px}
    .meta{display:block;font-size:11px;opacity:.7;margin-top:4px}
    .chat-send{display:flex;gap:8px; padding:10px; border-top:1px solid var(--stroke); background:#faf7f2}
    .chat-send input{flex:1;border:1px solid #e5e7eb;border-radius:10px;padding:10px}
    .chat-send button{border:1px solid var(--stroke);background:#111827;color:#fff;border-radius:10px;padding:10px 14px}
  </style>

  <div class="chat-wrap">
    <div class="chat-head">Soporte</div>
    <div id="chat" class="chat-body" aria-live="polite"></div>
    <div class="chat-send">
      <input id="txt" maxlength="2000" placeholder="Escribí tu mensaje…" />
      <button id="btnSend" type="button">Enviar</button>
    </div>
  </div>

  <asp:HiddenField ID="hfThreadId" runat="server" />
  <asp:HiddenField ID="hfIsAdmin" runat="server" Value="false" />

  <script>
    (function(){
      var chat = document.getElementById('chat');
      var txt  = document.getElementById('txt');
      var tid  = parseInt('<%= hfThreadId.Value %>',10) || 0;
      var last = 0; // último Id recibido

      function esc(s){ return (s||'').replace(/[<&>]/g, c=>({'<':'&lt;','>':'&gt;','&':'&amp;'}[c])); }
      function add(msg){
        var div = document.createElement('div');
        div.className = 'msg ' + (msg.isAdmin ? 'theirs' : 'mine');
        div.innerHTML = esc(msg.body) + '<span class="meta">' + new Date(msg.createdUtc).toLocaleString() + '</span>';
        chat.appendChild(div);
        chat.scrollTop = chat.scrollHeight;
      }

      function jpost(method, body){
        return fetch('SupportChat.aspx/' + method, {
          method:'POST',
          headers:{'Content-Type':'application/json; charset=utf-8'},
          body: JSON.stringify(body || {})
        }).then(r=>r.json()).then(p=>p.d||p);
      }

      function poll(){
        if(!tid) return;
        jpost('GetSince', { threadId: tid, sinceId: last })
          .then(d=>{
            (d.items||[]).forEach(m=>{ last = Math.max(last, m.id); add(m); });
          })
          .catch(()=>{})
          .finally(()=> setTimeout(poll, 3000));
      }

      document.getElementById('btnSend').onclick = function(){
        var t = (txt.value||'').trim(); if(!t) return;
        jpost('Send', { threadId: tid, text: t })
          .then(d=>{
            if(d && d.ok){ txt.value=''; /* el propio aparecerá en el siguiente poll */ }
          });
      };
      txt.addEventListener('keydown', function(e){ if(e.key==='Enter'){ e.preventDefault(); document.getElementById('btnSend').click(); } });

      poll();
    })();
  </script>
</asp:Content>
