<%@ Page Title="Reportes (Admin)" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="AdminReports.aspx.cs" Inherits="AdminReports" %>



<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <style>
  .reports-wrap{max-width:1200px;margin:16px auto;padding:0 12px}
  .card{background:#fff;border:1px solid #e5e7eb;border-radius:12px;padding:14px;margin-bottom:16px}

  .chart     {position:relative;width:100%;height:360px}  /* antes 280 */
  .chart-sm  {position:relative;width:100%;height:220px}  /* detalle preguntas */

  .grid-4{display:grid;grid-template-columns:1fr 1fr;gap:14px}
  @media (max-width: 1024px){ .grid-4{grid-template-columns:1fr} }
  canvas{display:block;max-width:100%}


  button{padding:8px 12px;border-radius:8px;border:1px solid #e5e7eb;background:#f9fafb}
</style>
  <h2>Reportes</h2>
    <div class="reports-wrap">

  <!-- ========= ENCUESTAS ========= -->
  <div class="card" style="margin-bottom:16px">
    <h3>Encuestas — estadísticas y comparaciones</h3>
    <div style="display:flex;gap:12px;align-items:flex-start;flex-wrap:wrap">
      <div>
        <div><b>Seleccionar encuestas</b></div>
        <div id="survList" style="min-width:280px;border:1px solid #eee;padding:8px;border-radius:8px;max-height:240px;overflow:auto"></div>
        <button type="button" id="btnLoadSurv">Ver</button>
      </div>
      <div style="flex:1">
        <div class="chart"><canvas id="chSurvTotals"></canvas></div>
        <div id="survPerSurvey" style="margin-top:12px"></div>
      </div>
    </div>
  </div>
   </div>

  <!-- ========= GANANCIAS ========= -->
  <div class="card">
    <h3>Ganancias</h3>
    <div style="display:flex;gap:12px;align-items:center;flex-wrap:wrap">
      <label>Desde <input type="date" id="revFrom"></label>
      <label>Hasta <input type="date" id="revTo"></label>
      <label>Moneda <input id="revCurr" placeholder="USD (vacío = todas)" style="width:100px" maxlength="3" pattern="^[A-Za-z]{3}$" title="Código de moneda de 3 letras"></label>
      <button type="button" id="btnLoadRev">Cargar</button>
    </div>
    <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px;margin-top:12px">
     <div class="grid-4" style="margin-top:12px">
  <div class="chart"><canvas id="chYear"></canvas></div>
  <div class="chart"><canvas id="chMonth"></canvas></div>
  <div class="chart"><canvas id="chWeek"></canvas></div>
  <div class="chart"><canvas id="chDay"></canvas></div>
</div>
    </div>
  </div>

  <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
 <script>
     function $id(id) { return document.getElementById(id); }
     function jpost(url, body) {
         return fetch(url, {
             method: 'POST',
             headers: { 'Content-Type': 'application/json; charset=utf-8' },
             body: body ? JSON.stringify(body) : '{}'
         }).then(r => r.json()).then(p => p.d || p);
     }

     // ===== ENCUESTAS =====
     let chTotals; const perSurveyCharts = [];
     const surveyTitleById = {};

     function loadSurveyList() {
         const url = '<%= ResolveUrl("~/EncuestasAdmin.aspx/List") %>';
         jpost(url).then(d => {
             const items = (d && d.items) ? d.items : [];
             const box = $id('survList'); box.innerHTML = '';
             if (items.length === 0) { box.innerHTML = '<em>No hay encuestas</em>'; return; }
             items.forEach(it => {
                 surveyTitleById[it.id] = it.title || ('Encuesta ' + it.id);
                 box.insertAdjacentHTML('beforeend',
                     `<label style="display:block;margin:4px 0">
             <input type="checkbox" value="${it.id}"> ${surveyTitleById[it.id]} ${it.isActive ? '(activa)' : ''}
           </label>`);
             });
         }).catch(err => console.error('List error', err));
     }

     function drawTotals(rows) {
         if (chTotals) chTotals.destroy();
         const labels = rows.map(r => r.title);
         const data = rows.map(r => r.total);
         chTotals = new Chart($id('chSurvTotals'), {
             type: 'bar',
             data: { labels, datasets: [{ label: 'Respuestas', data }] },
             options: {
                 responsive: true, maintainAspectRatio: false,
                 plugins: { legend: { display: false } },
                 scales: { y: { beginAtZero: true } }
             }
         });
     }

     function drawPerSurvey(sId, stats) {
         const host = document.createElement('div');
         host.style.marginTop = '10px';
         host.innerHTML = `<h4>${surveyTitleById[sId] || ('Encuesta ' + sId)}</h4>`;
         $id('survPerSurvey').appendChild(host);

         const items = (stats && stats.items) ? stats.items : [];
         if (items.length === 0) {
             host.insertAdjacentHTML('beforeend', '<em>Sin datos</em>'); return;
         }

         items.forEach(s => {
             const wrap = document.createElement('div'); wrap.className = 'chart-sm';
             const c = document.createElement('canvas'); wrap.appendChild(c);
             host.appendChild(wrap);

             if (s.qType === 1) { // Sí/No
                 perSurveyCharts.push(new Chart(c, {
                     type: 'pie',
                     data: { labels: ['Sí', 'No'], datasets: [{ data: [s.yes || 0, s.no || 0] }] },
                     options: {
                         responsive: true, maintainAspectRatio: false,
                         plugins: { title: { display: true, text: s.text || ('Q' + s.qIndex) } }
                     }
                 }));
             } else {           // Rating 1..5
                 perSurveyCharts.push(new Chart(c, {
                     type: 'bar',
                     data: {
                         labels: ['1', '2', '3', '4', '5'],
                         datasets: [{ data: [s.c1 || 0, s.c2 || 0, s.c3 || 0, s.c4 || 0, s.c5 || 0] }]
                     },
                     options: {
                         responsive: true, maintainAspectRatio: false,
                         plugins: { title: { display: true, text: s.text || ('Q' + s.qIndex) }, legend: { display: false } },
                         scales: { y: { beginAtZero: true } }
                     }
                 }));
             }
         });
     }

     function totalFromStats(stats) {
         let tot = 0;
         const items = (stats && stats.items) ? stats.items : [];
         items.forEach(s => {
             if (s.qType === 1) tot += (s.yes || 0) + (s.no || 0);
             else tot += (s.c1 || 0) + (s.c2 || 0) + (s.c3 || 0) + (s.c4 || 0) + (s.c5 || 0);
         });
         return tot;
     }

     $id('btnLoadSurv').onclick = function () {
         if (chTotals) chTotals.destroy();
         perSurveyCharts.splice(0).forEach(c => c.destroy());
         $id('survPerSurvey').innerHTML = '';

         const ids = [...$id('survList').querySelectorAll('input[type=checkbox]:checked')]
             .map(x => parseInt(x.value, 10));
         if (ids.length === 0) { console.warn('No hay encuestas seleccionadas'); return; }

         const totals = [];
         Promise.all(ids.map(id => {
             const url = '<%= ResolveUrl("~/EncuestasAdmin.aspx/Stats") %>';
      return jpost(url,{surveyId:id}).then(d=>{
        console.log('Stats', id, d);
        drawPerSurvey(id, d);
        totals.push({ id, title: surveyTitleById[id]||('Encuesta '+id), total: totalFromStats(d) });
      });
    })).then(()=>drawTotals(totals))
      .catch(err=>console.error('Stats error', err));
  };

  // ===== REVENUE =====
  function loadRevenue(gran, lab, cvsId){
    const from=$id('revFrom').value, to=$id('revTo').value, cur=$id('revCurr').value||null;
    return jpost('<%= ResolveUrl("~/AdminReports.aspx/Revenue") %>',
             { granularity: gran, fromUtc: from ? from + 'T00:00:00Z' : null, toUtc: to ? to + 'T23:59:59Z' : null, currency: cur })
             .then(d => {
                 const labels = (d.items || []).map(x => x.label);
                 const data = (d.items || []).map(x => x.amount);
                 return new Chart($id(cvsId), {
                     type: 'line',
                     data: { labels, datasets: [{ label: lab, data }] },
                     options: {
                         responsive: true, maintainAspectRatio: false,
                         plugins: { legend: { display: false } },
                         scales: { y: { beginAtZero: true } }
                     }
                 });
             });
     }

     let chY, chM, chW, chD;
     $id('btnLoadRev').onclick = function () {
         if (chY) chY.destroy(); if (chM) chM.destroy(); if (chW) chW.destroy(); if (chD) chD.destroy();
         Promise.all([
             loadRevenue('YEAR', 'Año', 'chYear').then(c => chY = c),
             loadRevenue('MONTH', 'Mes', 'chMonth').then(c => chM = c),
             loadRevenue('WEEK', 'Semana', 'chWeek').then(c => chW = c),
             loadRevenue('DAY', 'Día', 'chDay').then(c => chD = c)
         ]);
     };

     // init
     loadSurveyList();
 </script>

</asp:Content>
