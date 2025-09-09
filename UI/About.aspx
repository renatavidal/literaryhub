<%@ Page Title="¿Quiénes somos?" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="About" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" rel="stylesheet" href="/Content/carousel.css" />
</asp:Content>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server"><%: GetLocalResourceObject("PageTitle") %></asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="page">
    <h1><%: GetLocalResourceObject("H1") %></h1>

    <h2><%: GetLocalResourceObject("Sec1_Title") %></h2>
    <p><%: GetLocalResourceObject("Sec1_Body") %></p>
<div class="carousel" data-autoplay="true" data-interval="2000" aria-label= Carousel_Aria >
  <div class="carousel-track">
    <img id="slide1" src="/images/authorevent.jpg" alt="Evento con autores" class="carousel-img" />
    <img id="slide2" src="/images/bookclub1.jpg"  alt="Club de lectura"    class="carousel-img" />
    <img id="slide3" src="/images/books.jpg"       alt="Libros"             class="carousel-img" />
  </div>

  <button type="button" class="carousel-btn prev" aria-label= Carousel_Prev>&#10094;</button>
<button type="button" class="carousel-btn next" aria-label= Carousel_Next>&#10095;</button>

  <div class="carousel-dots" role="tablist" aria-label= Carousel_Dots >
  <button type="button" role="tab" aria-controls="slide1" aria-label="Imagen 1" class="active"></button>
  <button type="button" role="tab" aria-controls="slide2" aria-label="Imagen 2"></button>
  <button type="button" role="tab" aria-controls="slide3" aria-label="Imagen 3"></button>
</div>
</div>

    <h2><%: GetLocalResourceObject("Sec2_Title") %></h2>
    <p><%: GetLocalResourceObject("Sec2_Body") %></p>

    <h2><%: GetLocalResourceObject("Sec3_Title") %></h2>
    <ul>
      <li><%= GetLocalResourceObject("Sec3_Item1") %></li>
      <li><%= GetLocalResourceObject("Sec3_Item2") %></li>
      <li><%= GetLocalResourceObject("Sec3_Item3") %></li>
      <li><%= GetLocalResourceObject("Sec3_Item4") %></li>
      <li><%= GetLocalResourceObject("Sec3_Item5") %></li>
    </ul>

    <h2><%: GetLocalResourceObject("Sec4_Title") %></h2>
    <ul>
      <li><%: GetLocalResourceObject("Sec4_Item1") %></li>
      <li><%: GetLocalResourceObject("Sec4_Item2") %></li>
      <li><%: GetLocalResourceObject("Sec4_Item3") %></li>
      <li><%: GetLocalResourceObject("Sec4_Item4") %></li>
      <li><%: GetLocalResourceObject("Sec4_Item5") %></li>
      <li><%: GetLocalResourceObject("Sec4_Item6") %></li>
      <li><%: GetLocalResourceObject("Sec4_Item7") %></li>
    </ul>

    <h2><%: GetLocalResourceObject("Sec5_Title") %></h2>
    <p><%: GetLocalResourceObject("Sec5_Body") %></p>
  </div>
    <script src="<%= ResolveUrl("/Scripts/carousel.js") %>"></script>
</asp:Content>
