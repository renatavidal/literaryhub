<%@ Page Title="My Account" Language="C#" MasterPageFile="~/site.master"
    AutoEventWireup="true" CodeFile="MyAccount.aspx.cs" Inherits="MyAccount" %>


<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
  <style>
    .account { max-width:820px; margin:24px auto; display:grid; gap:18px }
    .card{ background:#fff; border:1px solid var(--stroke); border-radius:14px; padding:16px }
    .form-row{ display:grid; gap:6px }
    .grid-2{ display:grid; grid-template-columns:1fr 1fr; gap:16px }
    @media(max-width:780px){ .grid-2{ grid-template-columns:1fr } }
    .btn{ padding:10px 14px; border-radius:10px; border:1px solid var(--stroke); background:var(--bg-soft) }
    .btn-danger{ background:#fbeaea; border-color:#f2c2c2 }
    .table{ width:100%; border-collapse:collapse }
.table th,.table td{ padding:8px 10px; text-align:left; vertical-align:top; white-space:nowrap }
.table th:nth-child(3), .table td:nth-child(3){ white-space:normal } /* Título puede romper línea */
  </style>
    </asp:Content>
    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

  <div class="account">
    <h1><%: GetLocalResourceObject("Acc_Title") %></h1>

    <div class="card">
      <h3>Cambiar contraseña</h3>
      <p>Para actualizar tu contraseña usá el flujo seguro.</p>
      <asp:Button ID="btnGoToForgot" runat="server" Text="Ir a Cambiar Contraseña"
                  CssClass="btn" OnClick="btnGoToForgot_Click" />
      <asp:Label ID="lblPwdMsg" runat="server" />
    </div>

     <div class="card">
      <h3><%: GetLocalResourceObject("Acc_Lists_Title") %></h3>

      <h4><%: GetLocalResourceObject("Acc_Lists_Want") %></h4>
      <asp:Repeater ID="repWant" runat="server" OnItemCommand="repWant_ItemCommand">
        <ItemTemplate>
          <div class="book">
            <img src='<%# Eval("ThumbnailUrl") %>' alt="cover" style="width:48px;height:72px;object-fit:cover;border-radius:6px" />
            <b><%# Eval("Title") %></b> — <%# Eval("Authors") %>
            <div style="float:right">
              <asp:Button runat="server" CssClass="btn" Text="Marcar leído"
                CommandName="MoveToRead" CommandArgument='<%# Eval("BookId") %>' />
              <asp:Button runat="server" CssClass="btn" Text="<%$ Resources: Acc_Remove %>"
                CommandName="Remove" CommandArgument='<%# Eval("BookId") %>' />
            </div>
          </div>
        </ItemTemplate>
      </asp:Repeater>

      <hr />

      <h4>Leídos</h4>
      <asp:Repeater ID="repRead" runat="server" OnItemCommand="repRead_ItemCommand">
        <ItemTemplate>
          <div class="book">
            <img src='<%# Eval("ThumbnailUrl") %>' alt="cover" style="width:48px;height:72px;object-fit:cover;border-radius:6px" />
            <b><%# Eval("Title") %></b> — <%# Eval("Authors") %>
            <div style="float:right">
              <asp:Button runat="server" CssClass="btn" Text="<%$ Resources: Acc_Remove %>"
                CommandName="Remove" CommandArgument='<%# Eval("BookId") %>' />
            </div>
          </div>
        </ItemTemplate>
      </asp:Repeater>
    </div>

    <div class="card">
      <h3><%: GetLocalResourceObject("Acc_Deactivate_Title") %></h3>
      <p>Esto desactiva tu cuenta y oculta tu perfil. Podrás reactivarla contactando soporte.</p>
      <asp:Button ID="btnDeactivate" runat="server" Text="<%$ Resources: Acc_Deactivate_Button %>" CssClass="btn btn-danger" OnClientClick="return confirm('<%$ Resources: Acc_Deactivate_Confirm %>');" OnClick="btnDeactivate_Click" />
      <asp:Label ID="lblDeactivateMsg" runat="server" />
    </div>

<div class="card">
  <h3>Cuenta corriente</h3>
  <div class="grid-2">
    <div class="form-row">
      <label>Saldo actual</label>
      <asp:Label ID="lblSaldo" runat="server" CssClass="pill" />
    </div>
  </div>

  <h4 style="margin-top:12px">Movimientos</h4>
  <asp:GridView ID="gvCuenta" runat="server" AutoGenerateColumns="true"
                GridLines="None" CssClass="table"></asp:GridView>

  <h4 style="margin-top:16px">Notas</h4>
  <asp:GridView ID="gvNotas" runat="server" AutoGenerateColumns="true"
                GridLines="None" CssClass="table"></asp:GridView>
</div>

<div class="card">
  <h3>Mis compras</h3>
  <asp:GridView ID="gvCompras" runat="server" AutoGenerateColumns="false"
              CssClass="table" GridLines="None"
              DataKeyNames="PurchaseId"
              OnRowCommand="gvCompras_RowCommand">
  <Columns>
    <asp:BoundField DataField="PurchaseId" HeaderText="ID" ItemStyle-Width="70px" />
    <asp:BoundField DataField="BookId"     HeaderText="BookId" ItemStyle-Width="80px" />
    <asp:BoundField DataField="Title"      HeaderText="Título" />
    <asp:TemplateField HeaderText="Precio" ItemStyle-Width="120px">
      <ItemTemplate>
        <%# Eval("Price","{0:0.##}") %> <%# Eval("Currency") %>
      </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField DataField="CreatedUtc" HeaderText="Fecha (UTC)"
                    DataFormatString="{0:yyyy-MM-dd HH:mm}" ItemStyle-Width="150px" />
    <asp:TemplateField HeaderText="">
      <ItemTemplate>
        <asp:LinkButton runat="server" Text="Devolución" CommandName="Refund"
          CommandArgument='<%# Eval("PurchaseId") + "|" + Eval("BookId") + "|" + Eval("Title") %>'
          OnClientClick="return confirm('¿Generar nota de crédito por esta compra?');" />
      </ItemTemplate>
    </asp:TemplateField>
  </Columns>
</asp:GridView>


</div>

  </div>
</asp:Content>

