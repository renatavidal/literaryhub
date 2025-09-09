<%@ Page Title="Libro" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="BookDetails.aspx.cs" Inherits="BookDetails" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .book-wrap{display:grid;grid-template-columns:160px 1fr;gap:18px;align-items:start}
    .book-cover{width:160px;height:auto;border-radius:10px;border:1px solid var(--stroke)}
    .book-meta{display:grid;gap:6px}
    .book-title{font-size:1.3rem;font-weight:700;margin:0}
    .book-auth{color:#555}
    .actions{display:flex;gap:10px;flex-wrap:wrap;margin:10px 0 2px}
    .btn{padding:9px 14px;border-radius:10px;border:1px solid var(--stroke);background:#f6f3ef;cursor:pointer}
    .btn.primary{background:#c9a97a;color:#2b1e10;border-color:#b89567}
    .comment{display:grid;gap:6px;margin-top:14px}
    .input, .textarea{padding:10px;border:1px solid var(--stroke);border-radius:10px;background:#fff}
    .hint{font-size:.9rem;color:#666}
    .status{margin-top:8px}
  </style>
  <script>
    // Trae datos de Google Books y completa la UI + hidden fields
    async function loadBook(){
      const gid = new URLSearchParams(location.search).get("gid");
      if(!gid){ return; }
      const r = await fetch("https://www.googleapis.com/books/v1/volumes/"+encodeURIComponent(gid));
      if(!r.ok){ return; }
      const v = await r.json();
      const info = v.volumeInfo || {};
      const img = (info.imageLinks && (info.imageLinks.thumbnail || info.imageLinks.smallThumbnail)) || "";

      document.getElementById("imgCover").src = img || "/Content/blank-cover.png";
      document.getElementById("litTitle").textContent = info.title || "<%= GetLocalResourceObject("Book_Untitled") %>";
      document.getElementById("litAuthors").textContent = (info.authors||[]).join(", ");
      document.getElementById("litPub").textContent = info.publishedDate || "";
      document.getElementById("litDesc").textContent = (info.description||"").replace(/<[^>]+>/g,"").slice(0,600);

      // Hidden para el server
      document.getElementById("<%=hidGid.ClientID%>").value = gid;
      document.getElementById("<%=hidTitle.ClientID%>").value = info.title || "";
      document.getElementById("<%=hidAuthors.ClientID%>").value = (info.authors||[]).join(", ");
      document.getElementById("<%=hidThumb.ClientID%>").value = img || "";
      document.getElementById("<%=hidIsbn13.ClientID%>").value = (info.industryIdentifiers||[]).filter(x=>x.type==="ISBN_13").map(x=>x.identifier)[0] || "";
      document.getElementById("<%=hidPub.ClientID%>").value = info.publishedDate || "";
    }
    document.addEventListener("DOMContentLoaded", loadBook);
  </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <!-- Hidden fields que el code-behind usa para persistir -->
  <asp:HiddenField ID="hidGid" runat="server" />
  <asp:HiddenField ID="hidTitle" runat="server" />
  <asp:HiddenField ID="hidAuthors" runat="server" />
  <asp:HiddenField ID="hidThumb" runat="server" />
  <asp:HiddenField ID="hidIsbn13" runat="server" />
  <asp:HiddenField ID="hidPub" runat="server" />

  <div class="book-wrap">
    <img id="imgCover" class="book-cover" alt=" Book_CoverAlt" src="/Content/blank-cover.png" />
    <div class="book-meta">
      <h1 class="book-title"><span id="litTitle"><%: GetLocalResourceObject("Book_Loading") %></span></h1>
      <div class="book-auth"><strong><%: GetLocalResourceObject("Book_AuthorsLabel") %></strong> <span id="litAuthors"></span></div>
      <div class="hint"><strong><%: GetLocalResourceObject("Book_PublishedLabel") %></strong> <span id="litPub"></span></div>
      <p id="litDesc" class="hint"></p>

      <div class="actions">
        <asp:Button ID="btnWant" runat="server" Text="<%$ Resources: Book_Want %>" class="btn"
            OnClick="btnWant_Click" />
        <asp:Button ID="btnRead" runat="server" Text="<%$ Resources: Book_Read %>" class="btn"
            OnClick="btnRead_Click" />
      </div>

      <div class="comment">
        <label for="txtComment"><%: GetLocalResourceObject("Book_Comment_Label") %></label>
        <asp:TextBox ID="txtComment" runat="server" class="textarea" TextMode="MultiLine" Rows="4" MaxLength="2000"></asp:TextBox>
        <div class="hint"><%: GetLocalResourceObject("Book_Comment_Hint") %></div>
        <asp:Button ID="btnComment" runat="server" Text="<%$ Resources: Book_Comment_Publish %>" class="btn primary"
            OnClick="btnComment_Click" />
      </div>

      <asp:ValidationSummary ID="valSum" runat="server" ShowMessageBox="false" class="hint" />
      <asp:Literal ID="litStatus" runat="server" ></asp:Literal>
    </div>
  </div>
</asp:Content>


