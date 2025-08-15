<%@ Page Title="¿Quiénes somos?" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="About" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" rel="stylesheet" href="/Content/carousel.css" />
</asp:Content>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">¿Quiénes somos?</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="page">
    <h1>Sobre LiteraryHub</h1>

    <h2>Descripción básica del negocio</h2>
    <p>
      LiteraryHub conecta lectores, autores y librerías en un ecosistema único: recomendaciones
      personalizadas por algoritmos, comunidades de lectura virtuales, eventos exclusivos con autores
      y un marketplace para comprar libros en formato digital y físico con librerías asociadas.
      Incluye funciones sociales (perfiles, reseñas, seguimiento de autores) y un plan premium con
      beneficios y descuentos.
    </p>
<div class="carousel" data-autoplay="true" data-interval="2000" aria-label="Galería de imágenes">
  <div class="carousel-track">
    <img id="slide1" src="/images/authorevent.jpg" alt="Evento con autores" class="carousel-img" />
    <img id="slide2" src="/images/bookclub1.jpg"  alt="Club de lectura"    class="carousel-img" />
    <img id="slide3" src="/images/books.jpg"       alt="Libros"             class="carousel-img" />
  </div>

  <button type="button" class="carousel-btn prev" aria-label="Anterior">&#10094;</button>
<button type="button" class="carousel-btn next" aria-label="Siguiente">&#10095;</button>

  <div class="carousel-dots" role="tablist" aria-label="Selector de imagen">
  <button type="button" role="tab" aria-controls="slide1" aria-label="Imagen 1" class="active"></button>
  <button type="button" role="tab" aria-controls="slide2" aria-label="Imagen 2"></button>
  <button type="button" role="tab" aria-controls="slide3" aria-label="Imagen 3"></button>
</div>
</div>

    <h2>Situación actual</h2>
    <p>
      Startup emergente con equipo fundador de tecnología, marketing digital y editorial, asesorado por
      referentes del mundo literario. Visión: ser el hub digital literario líder en 3 años, con
      500.000 usuarios activos/mes, +200 librerías asociadas y +500 autores reconocidos.
      Lanzamiento local y expansión a 3 países el segundo año.
    </p>

    <h2>¿Qué nos hace únicos?</h2>
    <ul>
      <li><strong>Enfoque integral:</strong> recomendaciones + compra + comunidades en un solo lugar.</li>
      <li><strong>Hibridación digital–física:</strong> integra librerías locales y eventos presenciales.</li>
      <li><strong>Recomendaciones avanzadas:</strong> patrones de lectura, progreso y valoraciones contextualizadas.</li>
      <li><strong>Experiencia social enriquecida:</strong> comentarios por página evitando spoilers.</li>
      <li><strong>Puente autores–lectores:</strong> exposición y eventos que mejoran el ingreso del autor.</li>
    </ul>

    <h2>Factores clave de éxito</h2>
    <ul>
      <li>Tecnología de personalización que mejora con el uso.</li>
      <li>Modelo de ingresos diversificado (comisiones, suscripción, eventos, publicidad selectiva).</li>
      <li>Alianzas con editoriales, librerías, Kindle y creadores de contenido.</li>
      <li>Foco en comunidad para aumentar retención.</li>
      <li>Escalabilidad internacional (idiomas y contexto local).</li>
      <li>UX fluida entre descubrir, comprar, leer y debatir.</li>
      <li>Analytics útiles para autores y librerías.</li>
    </ul>

    <h2>Estrategia (Misión, Visión y marco temporal)</h2>
    <p>
      Horizonte de 3 años con evaluaciones trimestrales y metodología ágil: permite adaptarse a
      cambios económicos, tecnológicos y editoriales, incorporando IA, análisis predictivo y
      alianzas estratégicas sostenibles.
    </p>
  </div>
    <script src="<%= ResolveUrl("/Scripts/carousel.js") %>"></script>
</asp:Content>

