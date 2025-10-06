<%@ Page Title="Encuestas" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Encuestas.aspx.cs" Inherits="Encuestas" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <div id="box" class="cko" style="max-width:720px;margin:24px auto;padding:18px;background:#fff;border:1px solid #e5e7eb;border-radius:12px">
    <h2>Encuesta</h2>
    <div id="content">Cargando…</div>
    <div style="margin-top:12px;display:flex;gap:10px">
      <button id="btnSend" class="btn-send" style="border:none;background:#111827;color:#fff;padding:8px 14px;border-radius:8px">Enviar</button>
      <button id="btnSkip" class="btn-clear" style="border:none;background:#f3f4f6;padding:8px 14px;border-radius:8px">Omitir</button>
      <span id="msg"></span>
    </div>
  </div>

  <script>
    let currentSurvey = null;

    function renderSurvey(s){
      if(!s){ document.getElementById('content').textContent = "No hay encuestas pendientes. ¡Gracias!"; return; }
      currentSurvey = s;
      let html = `<h3>${s.title}</h3>`;
      s.questions.forEach(q=>{
        if(q.qType===1){ // Yes/No
          html += `
            <div style="margin-top:10px">
              <div><strong>Q${q.qIndex}.</strong> ${q.text}</div>
              <label><input type="radio" name="q${q.qIndex}" value="1"> Sí</label>
              <label style="margin-left:8px"><input type="radio" name="q${q.qIndex}" value="0"> No</label>
            </div>`;
        } else { // Rating 1..5
          html += `
            <div style="margin-top:10px">
              <div><strong>Q${q.qIndex}.</strong> ${q.text}</div>
              <select name="q${q.qIndex}">
                <option value="">(sin respuesta)</option>
                <option>1</option><option>2</option><option>3</option><option>4</option><option>5</option>
              </select>
            </div>`;
        }
      });
      document.getElementById('content').innerHTML = html;
    }

    function getAnswers(){
      const a = [null,null,null,null,null];
      if(!currentSurvey) return a;
      currentSurvey.questions.forEach(q=>{
        const name = "q"+q.qIndex;
        let v = null;
        if(q.qType===1){
          const sel = document.querySelector(`input[name=${name}]:checked`);
          if(sel) v = parseInt(sel.value,10);
        }else{
          const sel = document.querySelector(`select[name=${name}]`);
          if(sel && sel.value) v = parseInt(sel.value,10);
        }
        a[q.qIndex-1] = (v===0 || v>0) ? v : null;
      });
      return a;
    }

    function show(ok,msg){ const el = document.getElementById('msg'); el.style.color = ok?'#059669':'#b91c1c'; el.textContent=msg; setTimeout(()=>el.textContent='',2500); }

    function loadNext(){
      fetch('Encuestas.aspx/GetNext', {method:'POST', headers:{'Content-Type':'application/json; charset=utf-8'}})
       .then(r=>r.json()).then(p=>{
        const d = p.d || p;
        if(d && d.ok){ renderSurvey(d.survey); } else { renderSurvey(null); }
       }).catch(()=> renderSurvey(null));
    }

    document.getElementById('btnSend').onclick = function(){
      if(!currentSurvey){ return false; }
      const a = getAnswers();
      const body = JSON.stringify({ surveyId: currentSurvey.id, a1:a[0], a2:a[1], a3:a[2], a4:a[3], a5:a[4] });
      fetch('Encuestas.aspx/Submit', {method:'POST', headers:{'Content-Type':'application/json; charset=utf-8'}, body})
       .then(r=>r.json()).then(p=>{ const d=p.d||p; if(d.ok){ show(true,"¡Enviado!"); renderSurvey(null); } else { show(false,d.error||"Error"); }})
       .catch(()=>show(false,"Network error"));
      return false;
    };

    document.getElementById('btnSkip').onclick = function(){
      if(!currentSurvey){ return false;}
      const body = JSON.stringify({ surveyId: currentSurvey.id });
      fetch('Encuestas.aspx/Skip', {method:'POST', headers:{'Content-Type':'application/json; charset=utf-8'}, body})
       .then(r=>r.json()).then(p=>{ const d=p.d||p; if(d.ok){ show(true,"Omitida"); renderSurvey(null);} else { show(false,d.error||"Error"); }})
       .catch(()=>show(false,"Network error"));
      return false;
    };

    loadNext();
  </script>
</asp:Content>
