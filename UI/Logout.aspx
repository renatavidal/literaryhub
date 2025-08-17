<%@ Page Title="Cerrando sesión..." Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Logout.aspx.cs" Inherits="Logout" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Cerrando sesión…</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Cerrando sesión…</h1>
  <p>Te estamos redirigiendo. Si no ocurre automáticamente, <a href="/Landing.aspx">continuá aquí</a>.</p>
  <script>
    try { localStorage.removeItem('jwt'); } catch(e) {}
    setTimeout(function(){ window.location = '<%= ResolveUrl("/Landing.aspx") %>'; }, 200);
  </script>
</asp:Content>
