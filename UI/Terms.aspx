<%@ Page Title="Términos y condiciones" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Terms.aspx.cs" Inherits="Terms" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server"><%: GetLocalResourceObject("Terms_Title") %></asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: GetLocalResourceObject("Terms_H1") %></h1>
  <p><em>Última actualización: <%= DateTime.Now.ToString("yyyy-MM-dd") %></em></p>

  <h2>1. Aceptación</h2>
  <p>Al usar LiteraryHub aceptás estos términos. Si no estás de acuerdo, no utilices la plataforma.</p>

  <h2>2. Cuentas de usuario</h2>
  <ul>
    <li>Debés aportar información veraz y mantener la confidencialidad de tus credenciales.</li>
    <li>Sos responsable de la actividad que ocurra en tu cuenta.</li>
  </ul>

  <h2>3. Servicios</h2>
  <p>Incluyen: recomendaciones, comunidades, eventos con autores y marketplace de libros (digitales y físicos).</p>

  <h2>4. Compras y suscripciones</h2>
  <ul>
    <li>Los precios pueden variar por región. Los cargos de suscripción se renuevan automáticamente hasta que canceles.</li>
    <li>Las ventas de terceros (librerías asociadas) se rigen por sus políticas.</li>
  </ul>

  <h2>5. Contenido y conducta</h2>
  <ul>
    <li>Reseñas y comentarios deben cumplir normas de convivencia; se prohíbe contenido ilegal u ofensivo.</li>
    <li>No uses la plataforma para infringir derechos de autor.</li>
  </ul>

  <h2>6. Propiedad intelectual</h2>
  <p>Marcas, logos y software de LiteraryHub pertenecen a sus respectivos titulares.</p>

  <h2>7. Limitación de responsabilidad</h2>
  <p>La plataforma se ofrece “tal cual”. No garantizamos disponibilidad ininterrumpida. En ningún caso
     seremos responsables por daños indirectos o lucro cesante.</p>

  <h2>8. Modificaciones</h2>
  <p>Podemos actualizar estos términos; publicaremos la versión vigente en este sitio.</p>

  <h2>9. Contacto</h2>
  <p>Para dudas sobre estos términos, contactanos desde la sección <a href="/Contact.aspx">Contáctenos</a>.</p>
</asp:Content>

