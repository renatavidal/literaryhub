<%@ Page Title="Chats (Admin)" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="ChatsAdmin.aspx.cs" Inherits="ChatsAdmin" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <style>
    .chat-admin{display:grid;grid-template-columns:320px 1fr; gap:12px}
    .threads{background:#fff;border:1px solid var(--stroke);border-radius:12px;overflow:auto;max-height:72vh}
    .threads .item{padding:10px;border-bottom:1px solid #eee;cursor:pointer}
    .threads .item.active{background:#faf7f2}
    .pane{background:#fff;border:1px solid var(--stroke);border-radius:12px;display:flex;flex-direction:column;height:72vh}
    .pane-head{padding:10px;border-bottom:1px solid var(--stroke);display:flex;justify-content:space-between}
    .body{flex:1;overflow:auto;padding:12px;background:#f6f3ef}
    .msg{max-width:70%;margin:8px 0;padding:10px 12px;border-radius:14px;white-space:pre-wrap}
    .mine{background:#3b2f2a;color:#fff;margin-left:auto;border-bottom-right-radius:4px}
    .theirs{background:#fff;border:1px solid #e5e7eb;color:#1f2937;margin-right:auto;border-bottom-left-radius:4px}
    .send{display:flex;gap:8px;padding:10px;border-top:1px solid var(--stroke);background:#faf7f2}
    .send input{flex:1;border:1px solid #e5e7eb;border-radius:10px;padding:10px}
    .send button{border:1px solid var(--stroke);background:#111827;color:#fff;border-radius:10px;padding:10px 14px}
  </style>

  <div class="chat-admin">
    <div class="threads" id="list"></div>
    <div class="pane">
      <div class="pane-head">
        <div id="hdr">Seleccioná un chat</div>
        <button type="button" id="btnClose">Cerrar</button>
      </div>
      <div id="msgs" class="body"></div>
      <div class="send">
        <input id="txt" placeholder="Responder…" maxlength="2000" pattern=".{0,2000}" />
        <button type="button" id="btnSend">Enviar</button>
      </div>
    </div>
  </div>

  <script>
    (function(){
      var list = document.getElementById('list');
      var msgs = document.getElementById('msgs');
      var txt  = document.getElementById('txt');
      var hdr  = document.getElementById('hdr');
      var current = 0, last = 0;

      function esc(s){ return (s||'').replace(/[<&>]/g, c=>({'<':'&lt;','>':'&gt;','&':'&amp;'}[c])); }
      function api(m, body){
        return fetch('ChatsAdmin.aspx/'+m, {method:'POST', headers:{'Content-Type':'application/json; charset=utf-8'},
          body: JSON.stringify(body||{})}).then(r=>r.json()).then(p=>p.d||p);
      }

      function loadThreads(){
        api('List').then(d=>{
          list.innerHTML='';
          (d.items||[]).forEach(t=>{
            var el=document.createElement('div');
            el.className='item'+(t.id===current?' active':'');
            el.innerHTML='<b>#'+t.id+'</b> · user '+t.customerId+' · '+(t.status===1?'Abierto':'Cerrado')+
              '<div style="font-size:12px;opacity:.7">'+esc(t.lastMsg||'')+'</div>';
            el.onclick=function(){ openThread(t.id); };
            list.appendChild(el);
          });
        });
      }

      function render(m){
        var div=document.createElement('div');
        div.className='msg '+(m.isAdmin ? 'mine':'theirs');
        div.innerHTML=esc(m.body)+'<div class="meta" style="font-size:11px;opacity:.7;margin-top:4px">'+new Date(m.createdUtc).toLocaleString()+'</div>';
        msgs.appendChild(div); msgs.scrollTop=msgs.scrollHeight;
      }

      function poll(){
        if(!current) return setTimeout(poll, 2000);
        api('GetSince', {threadId: current, sinceId: last}).then(d=>{
          (d.items||[]).forEach(m=>{ last=Math.max(last,m.id); render(m); });
        }).finally(()=> setTimeout(poll, 2000));
      }

      function openThread(id){
        current=id; last=0; msgs.innerHTML=''; hdr.textContent='Chat #'+id;
        poll();
      }

      document.getElementById('btnSend').onclick=function(){
        var t=(txt.value||'').trim(); if(!t || !current) return;
        api('Send', {threadId: current, text: t}).then(()=>{ txt.value=''; });
      };
      document.getElementById('btnClose').onclick=function(){
        if(!current) return;
        api('Close', {threadId: current}).then(()=> loadThreads());
      };
      txt.addEventListener('keydown', function(e){ if(e.key==='Enter'){ e.preventDefault(); document.getElementById('btnSend').click(); } });

      loadThreads();
      setInterval(loadThreads, 7000);
      poll();
    })();
  </script>
</asp:Content>
