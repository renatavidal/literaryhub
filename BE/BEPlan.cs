namespace BE
{
    public class BEPlan { public int Id; public string Codigo; public string Descripcion; public string Roles; public decimal PrecioUSD; public bool Activo; public bool EsDestacado; }

    public class BESubscriptionOrder
    {
        public int Id;
        public int UserId;
        public int PlanId;
        public string Currency = "USD";
        public decimal TotalAmount;
        public string Status; // Pending/Paid/Failed/Cancelled
    }

    public class BEPaymentSplit
    {
        public string Method;  // "CARD" | "CREDIT_NOTE" | "ACCOUNT"
        public decimal Amount;
        // Datos tarjeta (sólo si Method == CARD)
        public string CardBrand;
        public string Last4;
        public byte[] PanEncrypted;
        public byte[] PanIV;
        // Referencia (NoteId, asiento c/c)
        public string RefId;
    }
}
