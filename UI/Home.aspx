<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="HomeBooks" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <style>
    .hero{background:#f5f0ea;border:1px solid #efe7df;border-radius:14px;padding:22px;margin:16px 0}
    .hero h1{font-family:Georgia,serif;color:#6b4226;margin:0 0 6px}
    .muted{color:#7a6b5e}

    .section{margin:28px 0}
    .section h2{font-family:Georgia,serif;color:#3b2f2a;margin:0 0 14px}
    .grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(160px,1fr));gap:14px}
    .card{background:#fff;border:1px solid #efe7df;border-radius:12px;overflow:hidden;box-shadow:0 2px 6px rgba(0,0,0,.06)}
    .cover{display:block;width:100%;height:220px;object-fit:cover;background:#fbf8f4}
    .card .body{padding:10px}
    .title{font-size:14px;font-weight:bold;color:#3b2f2a;line-height:1.2;height:34px;overflow:hidden}
    .author{font-size:12px;color:#7a6b5e;height:30px;overflow:hidden}
    .price{font-size:12px;color:#6b4226;margin-top:6px}
    .more{display:inline-block;margin-top:8px;font-size:12px;color:#a47148;text-decoration:none}
    .top-search{margin-top:10px;display:flex;gap:8px}
    .top-search input{flex:1;padding:10px;border:1px solid #ddd;border-radius:8px}
    .top-search button{padding:10px 14px;border:0;border-radius:8px;background:#a47148;color:#fff;cursor:pointer}
  </style>

  <div class="hero">
    <h1>Mantengamos la historia en marcha…</h1>
    <div class="muted">Explorá colecciones por género, seguí con tu lectura y descubrí novedades.</div>
    <div class="top-search">
      <asp:TextBox ID="txtSearch" runat="server" placeholder="Buscar libro, autor, edición…" />
      <asp:Button ID="btnSearch" runat="server" Text="Buscar" OnClick="btnSearch_Click" />
    </div>
  </div>

  <!-- Secciones por género -->
  <asp:Repeater ID="rptSections" runat="server">
    <ItemTemplate>
      <div class="section">
        <h2><%# Eval("Genre") %></h2>
        <div class="grid">
          <asp:Repeater ID="rptBooks" runat="server" DataSource='<%# Eval("Books") %>'>
            <ItemTemplate>
              <div class="card">
                <a href='<%# Eval("InfoLink") %>' target="_blank" rel="noopener">
                  <img class="cover" src='<%# Eval("Thumbnail") %>' alt='<%# Eval("Title") %>' />
                </a>
                <div class="body">
                  <div class="title"><%# Eval("Title") %></div>
                  <div class="author"><%# Eval("Authors") %></div>
                  <div class="price"><%# Eval("PriceLabel") %></div>
                  <a class="more" href='<%# Eval("InfoLink") %>' target="_blank" rel="noopener">Ver más →</a>
                </div>
              </div>
            </ItemTemplate>
          </asp:Repeater>
        </div>
      </div>
    </ItemTemplate>
  </asp:Repeater>
</asp:Content>
