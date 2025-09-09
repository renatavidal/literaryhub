<%@ Page Title="Cerrando sesión..." Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Logout.aspx.cs" Inherits="Logout" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server"><%: GetLocalResourceObject("Logout_Title") %></asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: GetLocalResourceObject("Logout_Heading") %></h1>
  <p><%: GetLocalResourceObject("Logout_Body") %> <a href="/Landing.aspx"><%: GetLocalResourceObject("Logout_Link") %></a>.</p>
  <script>
    try { localStorage.removeItem('jwt'); } catch(e) {}
    setTimeout(function(){ window.location = '<%= ResolveUrl("/Landing.aspx") %>'; }, 200);
  </script>
</asp:Content>

