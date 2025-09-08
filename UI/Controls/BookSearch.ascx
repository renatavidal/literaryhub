<%@ Control Language="C#" AutoEventWireup="true"
    CodeFile="BookSearch.ascx.cs" Inherits="Controls_BookSearch" %>

<style>
  /* ——— Reusa tu paleta ——— */
  :root{
    --stroke:#e8dacd; --ink:#3b2f2a; --muted:#7a6b5e;
    --brand-vibrant-rgb:195,122,66; /* terracota */
    --brand-deep-rgb:107,66,38;     /* marrón profundo */
  }

  /* ==== Search bar (pill) ==== */
  .search-wrap{ margin:18px 0 10px; display:flex; justify-content:center }
  .search-bar{
    --h:46px; display:flex; align-items:center; gap:10px;
    width:100%; max-width:560px; padding:6px 6px 6px 10px;
    border-radius:999px; background:linear-gradient(180deg,
      rgba(var(--brand-vibrant-rgb), .10), rgba(255,255,255,.86));
    border:1px solid var(--stroke);
    box-shadow: inset 0 1px 0 rgba(255,255,255,.7), 0 6px 16px rgba(0,0,0,.06);
    transition:border-color .18s, box-shadow .18s;
  }
  .search-bar:focus-within{
    border-color: rgba(var(--brand-vibrant-rgb), .45);
    box-shadow: 0 0 0 3px rgba(var(--brand-vibrant-rgb), .22), 0 10px 24px rgba(0,0,0,.08);
  }
  .search-ico{
    width:20px; height:20px; opacity:.65; color:#6b4226;
    -webkit-mask:url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M15.5 14h-.79l-.28-.27A6.5 6.5 0 1 0 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 5 1.5-1.5-5-5zM9.5 14C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/></svg>') center/contain no-repeat;
            mask:url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M15.5 14h-.79l-.28-.27A6.5 6.5 0 1 0 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 5 1.5-1.5-5-5zM9.5 14C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/></svg>') center/contain no-repeat;
    background: currentColor;
  }
  .search-input{ flex:1; height:var(--h); border:0; outline:0; background:transparent; padding:0 8px; font-size:15px; color:var(--ink) }
  .search-input::placeholder{ color:var(--muted); opacity:.7 }
  .search-btn{
    min-width:110px; height:calc(var(--h) - 12px); border:0; border-radius:999px;
    padding:0 16px; font-weight:700; color:#fff; cursor:pointer;
    background:linear-gradient(180deg, rgba(var(--brand-deep-rgb), .98), rgba(var(--brand-deep-rgb), .90));
    box-shadow:0 6px 16px rgba(var(--brand-deep-rgb), .28);
    transition:transform .06s ease;
  }
  .search-btn:hover{ transform:translateY(-1px) }

  /* ==== Cards / grid (idéntico a Home) ==== */
  .grid{ display:grid; grid-template-columns:repeat(auto-fill, minmax(200px,1fr)); gap:20px }
  .book-card{ position:relative; overflow:hidden; border-radius:16px;
    background:rgba(var(--brand-vibrant-rgb), .20); border:1px solid rgba(var(--brand-vibrant-rgb), .38);
    box-shadow:0 2px 12px rgba(0,0,0,.12); transition:transform .18s, box-shadow .18s;
  }
  .book-card:hover{ transform:translateY(-2px); box-shadow:0 14px 30px rgba(0,0,0,.18) }
  .cover-wrap{ background:linear-gradient(180deg, rgba(var(--brand-vibrant-rgb), .30), rgba(var(--brand-vibrant-rgb), .12)); padding:10px }
  .cover{ width:100%; height:200px; object-fit:cover; border-radius:12px; background:#fbf8f4 }
  .body{ padding:10px 12px 12px; display:grid; gap:6px }
  .book-title{ font-size:18px; line-height:1.2; color:#6b4226; margin:0 }
  .book-sub{ font-size:13px; color:#7a6b5e; margin:0 }
  .book-price{ font-size:12px; color:#6b4226; font-weight:700 }
  .dim-on-hover{ transition:filter .18s; position:relative; z-index:1 }
  .book-card:hover>.dim-on-hover{ filter:blur(1.5px) brightness(.92) }
  .overlay-actions{ position:absolute; inset:0; z-index:2; border-radius:inherit; display:flex; align-items:center; justify-content:center;
    background:rgba(20,14,9,.48); backdrop-filter:blur(4px) saturate(106%); opacity:0; pointer-events:none; transition:opacity .18s }
  .book-card:hover .overlay-actions{ opacity:1; pointer-events:auto }
  .action-stack{ display:flex; flex-direction:column; gap:10px; width:min(80%, 220px) }
  .btn, .btn:link, .btn:visited, .btn:hover, .btn:active{ color:#fff !important; text-decoration:none !important }
  .btn{ display:block; text-align:center; font-size:16px; padding:11px 14px; border:0; border-radius:14px;
    background:rgba(var(--brand-vibrant-rgb), .96); box-shadow:0 2px 0 rgba(0,0,0,.07), 0 0 0 1px rgba(255,255,255,.12) inset }
  .btn:hover{ background:rgba(var(--brand-vibrant-rgb), 1) }
  .btn-primary{ background:rgba(var(--brand-deep-rgb), .98); font-weight:700; box-shadow:0 8px 18px rgba(var(--brand-deep-rgb), .35) }

  .btn-row{ display:none }
  @media(hover:none){ .overlay-actions{ display:none!important } .btn-row{ display:flex; gap:8px; flex-wrap:wrap; justify-content:center; margin-top:8px } }
  .clamp-2{ display:-webkit-box; -webkit-line-clamp:2; -webkit-box-orient:vertical; overflow:hidden }
  .clamp-1{ display:-webkit-box; -webkit-line-clamp:1; -webkit-box-orient:vertical; overflow:hidden }
</style>

<div class="search-wrap">
  <div class="search-bar">
    <span class="search-ico" aria-hidden="true"></span>
    <asp:TextBox ID="txtQ" runat="server" CssClass="search-input" />
    <asp:Button ID="btnGo" runat="server" CssClass="search-btn"
      Text="<%$ Resources: Global, Btn_Search %>" OnClick="btnGo_Click" />
  </div>
</div>

<asp:Panel ID="pnlResults" runat="server" Visible="false">
  <div class="grid">
    <asp:Repeater ID="rpt" runat="server">
      <ItemTemplate>
        <div class="book-card">
          <div class="dim-on-hover">
            <div class="cover-wrap">
              <asp:HyperLink runat="server" NavigateUrl='<%# BookUrl(Eval("Gid")) %>'>
                <asp:Image runat="server" CssClass="cover"
                  ImageUrl='<%# Eval("Thumbnail") %>' AlternateText='<%# Eval("Title") %>' />
              </asp:HyperLink>
            </div>
            <div class="body">
              <h3 class="book-title clamp-2"><%# Eval("Title") %></h3>
              <p class="book-sub clamp-1"><%# Eval("Authors") %></p>
              <div class="book-price"><%# Eval("PriceLabel") %></div>

              <div class="btn-row">
                <asp:HyperLink ID="lnkView" runat="server" CssClass="btn"
    NavigateUrl='<%# ResolveClientUrl("/BookDetails.aspx?gid=") + Eval("Gid") %>'>
  <%# GetGlobalResourceObject("Global", "Btn_ViewComment") %>
</asp:HyperLink>

                <asp:LinkButton runat="server" CssClass="btn"
                  CommandName="want" CommandArgument='<%# Eval("Gid") %>'
                  OnCommand="BookAction_Command" CausesValidation="false" UseSubmitBehavior="false">
                  <%# GetGlobalResourceObject("Global", "Btn_Want") %>
                </asp:LinkButton>
                <asp:LinkButton runat="server" CssClass="btn"
                  CommandName="read" CommandArgument='<%# Eval("Gid") %>'
                  OnCommand="BookAction_Command" CausesValidation="false" UseSubmitBehavior="false">
                  <%# GetGlobalResourceObject("Global", "Btn_Read") %>
                </asp:LinkButton>
                <asp:HyperLink runat="server" CssClass="btn btn-primary"
                  NavigateUrl='<%# "/Purchase.aspx?gid=" + Eval("Gid") %>'>
                  <%# GetGlobalResourceObject("Global", "Btn_Buy") %>
                </asp:HyperLink>
              </div>
            </div>
          </div>

          <div class="overlay-actions">
            <div class="action-stack">
              <asp:HyperLink runat="server" CssClass="btn"
                NavigateUrl='<%# BookUrl(Eval("Gid")) %>'>
                <%# GetGlobalResourceObject("Global", "Btn_ViewComment") %>
              </asp:HyperLink>
              <asp:LinkButton runat="server" CssClass="btn"
                CommandName="want" CommandArgument='<%# Eval("Gid") %>'
                OnCommand="BookAction_Command" CausesValidation="false" UseSubmitBehavior="false">
                <%# GetGlobalResourceObject("Global", "Btn_Want") %>
              </asp:LinkButton>
              <asp:LinkButton runat="server" CssClass="btn"
                CommandName="read" CommandArgument='<%# Eval("Gid") %>'
                OnCommand="BookAction_Command" CausesValidation="false" UseSubmitBehavior="false">
                <%# GetGlobalResourceObject("Global", "Btn_Read") %>
              </asp:LinkButton>
              <asp:HyperLink runat="server" CssClass="btn btn-primary"
                NavigateUrl='<%# "/Purchase.aspx?gid=" + Eval("Gid") %>'>
                <%# GetGlobalResourceObject("Global", "Btn_Buy") %>
              </asp:HyperLink>
            </div>
          </div>
        </div>
      </ItemTemplate>
    </asp:Repeater>
  </div>
</asp:Panel>
