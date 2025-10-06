<%@ Page Title="Encuestas (Admin)" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="EncuestasAdmin.aspx.cs" Inherits="EncuestasAdmin" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <style>
  .mback{position:fixed;inset:0;background:rgba(0,0,0,.45);display:none;z-index:9999}
  .mbox{position:fixed;left:50%;top:50%;transform:translate(-50%,-50%);
        width:min(760px,92vw);background:#fff;border-radius:12px;padding:16px}
  .head{display:flex;justify-content:space-between;align-items:center}
  .row{display:grid;grid-template-columns:1fr 140px;gap:8px;margin-top:8px;align-items:center}
  .btn{border:none;border-radius:8px;padding:8px 12px;cursor:pointer}
  .btnp{background:#111827;color:#fff}.btns{background:#f3f4f6}
</style>

  <h2>Encuestas</h2>
  <div style="display:flex;gap:10px;align-items:center">
    <select id="ddl" style="min-width:320px"></select>
    <button type="button" id="btnLoad">Ver resultados</button>
    <button  type="button" id="btnNew">Crear nueva</button>
    <button type="button"  id="btnDel">Borrar</button>
  </div>
  <div id="chartZone" style="margin-top:16px"></div>
    <div id="newBack" class="mback">
  <div class="mbox">
    <div class="head">
      <h3 style="margin:0">Nueva encuesta</h3>
      <button type="button" class="btn btns" id="newClose">×</button>
    </div>

    <div style="margin-top:8px">
      <div style="display:flex;gap:10px;align-items:center">
        <label>Título <input id="svTitle" style="width:420px"/></label>
        <label><input type="checkbox" id="svActive" checked/> Activa</label>
      </div>

      <!-- 5 filas: texto + tipo (1=Sí/No, 2=Rating) -->
      <div class="row"><input id="txtQ1" placeholder="Pregunta 1 (opcional)"/>
        <select id="selQ1"><option value="">(tipo)</option><option value="1">Sí/No</option><option value="2">Rating 1..5</option></select></div>
      <div class="row"><input id="txtQ2" placeholder="Pregunta 2 (opcional)"/>
        <select id="selQ2"><option value="">(tipo)</option><option value="1">Sí/No</option><option value="2">Rating 1..5</option></select></div>
      <div class="row"><input id="txtQ3" placeholder="Pregunta 3 (opcional)"/>
        <select id="selQ3"><option value="">(tipo)</option><option value="1">Sí/No</option><option value="2">Rating 1..5</option></select></div>
      <div class="row"><input id="txtQ4" placeholder="Pregunta 4 (opcional)"/>
        <select id="selQ4"><option value="">(tipo)</option><option value="1">Sí/No</option><option value="2">Rating 1..5</option></select></div>
      <div class="row"><input id="txtQ5" placeholder="Pregunta 5 (opcional)"/>
        <select id="selQ5"><option value="">(tipo)</option><option value="1">Sí/No</option><option value="2">Rating 1..5</option></select></div>

      <div style="margin-top:12px;display:flex;gap:8px">
        <button type="button" class="btn btnp" id="newCreate">Crear</button>
        <button type="button" class="btn btns" id="newCancel">Cancelar</button>
        <span id="newMsg" style="font-weight:600"></span>
      </div>
    </div>
  </div>
</div>

  <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
  <script>
      const nb = document.getElementById('newBack');
      document.getElementById('btnNew').onclick = () => nb.style.display = 'block';
      document.getElementById('newClose').onclick = () => nb.style.display = 'none';
      document.getElementById('newCancel').onclick = () => nb.style.display = 'none';

      document.getElementById('newCreate').onclick = function () {
          const title = (document.getElementById('svTitle').value || '').trim();
          const isActive = document.getElementById('svActive').checked;
          if (!title) { alert('Título requerido'); return; }

          function t(i) { return (document.getElementById('txtQ' + i).value || '').trim() || null; }
          function y(i) { var v = document.getElementById('selQ' + i).value; return v ? parseInt(v, 10) : null; }

          // Requerimos al menos 1 pregunta con tipo
          var any = false;
          for (var i = 1; i <= 5; i++) { if (t(i) && y(i)) { any = true; break; } }
          if (!any) { alert('Cargá al menos 1 pregunta y su tipo.'); return; }

          const payload = {
              title, isActive,
              q1Text: t(1), q1Type: y(1), q2Text: t(2), q2Type: y(2), q3Text: t(3), q3Type: y(3),
              q4Text: t(4), q4Type: y(4), q5Text: t(5), q5Type: y(5)
          };

          fetch('EncuestasAdmin.aspx/Create', {
              method: 'POST', headers: { 'Content-Type': 'application/json; charset=utf-8' },
              body: JSON.stringify(payload)
          }).then(() => { nb.style.display = 'none'; loadList(); });
      };
    let chartRefs = [];

    function loadList(){
      fetch('EncuestasAdmin.aspx/List', {method:'POST', headers:{'Content-Type':'application/json; charset=utf-8'}})
       .then(r=>r.json()).then(p=>{
         const d = p.d || p; const ddl = document.getElementById('ddl'); ddl.innerHTML = '';
         (d.items||[]).forEach(it=>{
           const o = document.createElement('option'); o.value = it.id; o.textContent = `${it.title} ${it.isActive?'(activa)':''}`;
           ddl.appendChild(o);
         });
       });
    }

    function drawStats(stats){
      // limpio
      chartRefs.forEach(c=>c.destroy()); chartRefs = [];
      const zone = document.getElementById('chartZone'); zone.innerHTML = '';
      stats.forEach(s=>{
        const canvas = document.createElement('canvas'); zone.appendChild(canvas);
        if(s.qType===1){ // yes/no -> pie
          const data = {labels:['Sí','No'], datasets:[{data:[s.yes, s.no]}]};
          chartRefs.push(new Chart(canvas, {type:'pie', data}));
        } else { // rating -> bar 1..5
          const data = {labels:['1','2','3','4','5'], datasets:[{data:[s.c1,s.c2,s.c3,s.c4,s.c5]}]};
          chartRefs.push(new Chart(canvas, {type:'bar', data, options:{scales:{y:{beginAtZero:true}}}}));
        }
      });
    }

    document.getElementById('btnLoad').onclick = function(){
      const id = parseInt(document.getElementById('ddl').value||'0',10);
      if(!id) return;
      fetch('EncuestasAdmin.aspx/Stats', {method:'POST', headers:{'Content-Type':'application/json; charset=utf-8'}, body: JSON.stringify({ surveyId:id })})
        .then(r=>r.json()).then(p=>{
          const d = p.d || p;
          drawStats(d.items||[]);
        });
    };

    document.getElementById('btnDel').onclick = function(){
      const id = parseInt(document.getElementById('ddl').value||'0',10);
      if(!id) return;
      if(!confirm('¿Borrar encuesta?')) return;
      fetch('EncuestasAdmin.aspx/Delete', {method:'POST', headers:{'Content-Type':'application/json; charset=utf-8'}, body: JSON.stringify({ surveyId:id })})
        .then(()=>loadList());
    };

    

    loadList();
  </script>
</asp:Content>
