<%@ Page Title="Literary Hub" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Landing.aspx.cs" Inherits="Landing" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* Estilos específicos del landing (el resto viene de /Content/site.css) */
        .section { margin-bottom: 28px; }
        .cards { display:grid; grid-template-columns: repeat(4, minmax(0,1fr)); gap:16px; }
        .card {
            background: var(--paper);
            border: 1px solid var(--stroke);
            border-radius: 16px;
            padding: 18px;
            text-align: center;
            min-height: 150px;
            display:flex; flex-direction:column; justify-content:center; gap:8px;
        }
        .card .title { font-weight: 800; letter-spacing:.5px; font-size:1.15rem; }
        .card .author { color: var(--ink-soft); font-size:.95rem; }
        .chips { display:grid; grid-template-columns: 1fr 1fr; gap:12px; }
        .chip {
            background: var(--chip);
            border: 1px solid var(--stroke);
            border-radius: 12px;
            padding: 16px;
            text-align:center;
            font-weight: 600;
        }
        .features {
            display:grid; grid-template-columns: repeat(3, minmax(0,1fr)); gap:16px;
        }
        .feature {
            background: var(--paper);
            border: 1px solid var(--stroke);
            border-radius: 16px;
            padding: 18px;
        }
        .feature h3 { margin:0 0 8px 0; }
        .cta-row { display:flex; gap:12px; justify-content:center; flex-wrap:wrap; margin-top:12px; }
        @media (max-width: 980px){
            .cards { grid-template-columns: repeat(2, minmax(0,1fr)); }
            .features { grid-template-columns: 1fr; }
        }
    </style>
</asp:Content>



<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Héroe -->
    <section class="hero">
        <h1><%: GetLocalResourceObject("Landing_Hero_Title") %></h1>
        <p><%: GetLocalResourceObject("Landing_Hero_Sub") %></p>
        <div class="cta-row">
            <a runat="server" href="/Login.aspx" class="btn ghost"><%: GetLocalResourceObject("Landing_Hero_Login") %></a>
            <a runat="server" href="/Signup.aspx" class="btn solid"><%: GetLocalResourceObject("Landing_Hero_Signup") %></a>
        </div>
    </section>

    <!-- Libros populares -->
    <section class="section">
        <h2 style="margin:0 0 12px 0;"><%: GetLocalResourceObject("Landing_Popular_Title") %></h2>
        <div class="cards">
            <article class="card">
                <div class="title">The Great Gatsby</div>
                <div class="author">F. Scott Fitzgerald</div>
            </article>
            <article class="card">
                <div class="title"><em>Beloved</em></div>
                <div class="author">Toni Morrison</div>
            </article>
            <article class="card">
                <div class="title">Moby Dick</div>
                <div class="author">Herman Melville</div>
            </article>
            <article class="card">
                <div class="title">1984</div>
                <div class="author">George Orwell</div>
            </article>
        </div>
    </section>



<!-- Beneficios / Features -->
<section class="section">
  <div class="features">
    <div class="feature">
      <h3><%: GetLocalResourceObject("Landing_Feature1_Title") %></h3>
      <p><%: GetLocalResourceObject("Landing_Feature1_Body") %></p>
      <a runat="server" href="/Groups.aspx" class="btn ghost"><%: GetLocalResourceObject("Landing_Feature1_CTA") %></a>
    </div>
    <div class="feature">
      <h3><%: GetLocalResourceObject("Landing_Feature2_Title") %></h3>
      <p>Participá en sesiones de preguntas y respuestas (Q&amp;A), lanzamientos de libros e
         entrevistas exclusivas con tus autores favoritos.</p>
      <a runat="server" href="/Events.aspx" class="btn ghost"><%: GetLocalResourceObject("Landing_Feature2_CTA") %></a>
    </div>
    <div class="feature">
      <h3><%: GetLocalResourceObject("Landing_Feature3_Title") %></h3>
      <p><p><%: GetLocalResourceObject("Landing_Feature3_Body") %></p>
      <a runat="server" href="/Genres.aspx" class="btn ghost"><%: GetLocalResourceObject("Landing_Feature3_CTA") %></a>
    </div>
  </div>
</section>

<!-- CTA final -->
<section class="section" style="text-align:center;">
  <p style="color:var(--ink-soft);max-width:60ch;margin:0 auto 12px;">
    Sumate a miles de lectores que descubren nuevos libros, se unen a grupos de lectura
    y asisten a eventos en vivo con autores.
  </p>
    <div class="autor-libreria"> <a runat="server" href="/SignupCliente.aspx" class="signupcliente"><%: GetLocalResourceObject("Landing_AuthorBanner") %></a></div>
  <div class="cta-row">
    <a runat="server" href="/Login.aspx" class="btn ghost"><%: GetLocalResourceObject("Landing_CTA_Login") %></a>
    <a runat="server" href="/Signup.aspx" class="btn cta"><%: GetLocalResourceObject("Landing_CTA_Signup") %></a>
  </div>
</section>

</asp:Content>


