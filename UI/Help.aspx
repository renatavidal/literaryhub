<%@ Page Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="Help.aspx.cs" Inherits="Help"
    Title="<%$ Resources: Help_TitleTag %>" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <style>
    .container{max-width:900px;margin:40px auto;background:#fff;padding:30px;border-radius:12px;box-shadow:0 2px 6px rgba(0,0,0,.1)}
    h1{ text-align:center;margin-bottom:30px;font-family:Georgia,serif;color:#6b4226}
    .faq{ margin-bottom:20px}
    .faq h3{margin:0;font-size:18px;cursor:pointer;padding:10px;background:#f5f0ea;border-radius:6px}
    .faq p{margin:0;padding:10px;display:none;border-left:3px solid #a47148;background:#fbf8f4}
    .faq.active p{display:block}
    .back-link{display:block;margin-top:30px;text-align:center}
    .back-link a{text-decoration:none;color:#a47148;font-weight:bold}
  </style>

  <script>
      window.onload = function () {
          var items = document.querySelectorAll(".faq h3");
          for (var i = 0; i < items.length; i++) {
              items[i].addEventListener("click", function () {
                  this.parentElement.classList.toggle("active");
              });
          }
      };
  </script>

  <div class="container">
    <h1><%: GetLocalResourceObject("Help_H1") %></h1>

    <div class="faq">
      <h3><%: GetLocalResourceObject("Help_Q1") %></h3>
      <p><%: GetLocalResourceObject("Help_A1") %></p>
    </div>

    <div class="faq">
      <h3><%: GetLocalResourceObject("Help_Q2") %></h3>
      <p><%: GetLocalResourceObject("Help_A2") %></p>
    </div>

    <div class="faq">
      <h3><%: GetLocalResourceObject("Help_Q3") %></h3>
      <p><%: GetLocalResourceObject("Help_A3") %></p>
    </div>

    <div class="faq">
      <h3><%: GetLocalResourceObject("Help_Q4") %></h3>
      <p><%: GetLocalResourceObject("Help_A4") %></p>
    </div>

    <div class="faq">
      <h3><%: GetLocalResourceObject("Help_Q5") %></h3>
      <p><%: GetLocalResourceObject("Help_A5") %></p>
    </div>

    <div class="back-link">
      <a href="<%= ResolveUrl("~/Landing.aspx") %>"><%: GetLocalResourceObject("Help_Back") %></a>
    </div>
  </div>
</asp:Content>
