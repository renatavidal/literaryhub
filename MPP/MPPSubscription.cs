using System;
using System.Collections;
using System.Data;
using BE;
using DAL;

namespace MPP
{
    public class MPPSubscription
    {
        private readonly Acceso _db = new Acceso();

        public int BeginOrder(int userId, string planCode, string currency)
        {
            var hs = new Hashtable
            {
                { "@UserId", userId },
                { "@PlanCode", planCode },
                { "@Currency", currency }
            };
            // SP debe terminar con: SELECT SCOPE_IDENTITY();
            return _db.LeerCantidad("dbo.usp_Subscription_Begin", hs);
        }

        public void RegisterPayment(int orderId, BEPaymentSplit p)
        {
            var hs = new Hashtable
            {
                { "@OrderId", orderId },
                { "@Method",  p.Method },
                { "@Amount",  p.Amount }
            };

            // Solo si es tarjeta
            if (string.Equals(p.Method, "CARD", StringComparison.OrdinalIgnoreCase))
            {
                hs["@CardBrand"] = (object)p.CardBrand ?? DBNull.Value;
                hs["@CardLast4"] = (object)p.Last4 ?? DBNull.Value;
                if (p.PanEncrypted != null) hs["@PanEncrypted"] = p.PanEncrypted;   // byte[]
                if (p.PanIV != null) hs["@PanIV"] = p.PanIV;          // byte[]
            }

            // Solo si es Nota de Crédito
            if (string.Equals(p.Method, "CREDIT_NOTE", StringComparison.OrdinalIgnoreCase))
            {
                // RefId INT (NoteId)
                int noteId;
                if (!int.TryParse(Convert.ToString(p.RefId), out noteId))
                    throw new Exception("Nota de crédito inválida.");
                hs["@RefId"] = noteId;
            }

            if (!_db.Escribir("dbo.usp_Payment_Register", hs))
                throw new Exception("No se pudo registrar el pago.");
        }

        public decimal GetCreditNoteRemainingByUser(int noteId, int userId)
        {
            var dt = _db.Leer("dbo.s_Note_Remaining_ByUser",
                new Hashtable { { "@NoteId", noteId }, { "@UserId", userId } });
            if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value) return 0m;
            return Convert.ToDecimal(dt.Rows[0][0]);
        }

        public void ConfirmOrder(int orderId)
        {
            var hs = new Hashtable { { "@OrderId", orderId } };
            if (!_db.Escribir("dbo.usp_Subscription_Confirm", hs))
                throw new Exception("No se pudo confirmar la orden.");
        }

        public int CreateNote(int userId, char type, decimal amount, string reason)
        {
            var hs = new Hashtable
            {
                { "@UserId", userId },
                { "@Type", type },
                { "@Amount", amount },
                { "@Reason", (object)reason ?? DBNull.Value }
            };
            // SP debe terminar con: SELECT SCOPE_IDENTITY();
            return _db.LeerCantidad("dbo.usp_Note_Create", hs);
        }
        public BE.BEPlan GetPlanByCode(string code)
        {
            var hs = new System.Collections.Hashtable { { "@Codigo", code } };
            var dt = new DAL.Acceso().Leer("s_Plan_ByCode", hs);  // SP abajo
            if (dt.Rows.Count == 0) return null;

            var r = dt.Rows[0];
            var p = new BE.BEPlan();
            p.Id = Convert.ToInt32(r["Id"]);
            p.Codigo = Convert.ToString(r["Codigo"]);
            p.Descripcion = Convert.ToString(r["Descripcion"]);
            p.Roles = Convert.ToString(r["Roles"]);
            p.PrecioUSD = Convert.ToDecimal(r["PrecioUSD"]);
            return p;
        }
        public BEActiveSubscription GetActiveForUser(int userId)
        {
            var t = _db.Leer("usp_User_ActiveSubscription",
                             new Hashtable { { "@UserId", userId } });
            if (t.Rows.Count == 0) return null;

            var r = t.Rows[0];
            return new BEActiveSubscription
            {
                OrderId = Convert.ToInt32(r["OrderId"]),
                PlanId = Convert.ToInt32(r["PlanId"]),
                PlanName = Convert.ToString(r["PlanName"]),
                PaidUtc = r["PaidUtc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["PaidUtc"]),
                ExpiresUtc = r["ExpiresUtc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["ExpiresUtc"]),
                IsActive = Convert.ToInt32(r["IsActive"]) == 1
            };
        }
        public void CancelForUser(int userId)
        {
            var acc = new Acceso();
            acc.Escribir("usp_User_CancelSubscription", new Hashtable { { "@UserId", userId } });
        }


        public decimal GetCreditNoteRemaining(int noteId)
        {
            var dt = new DAL.Acceso().Leer("s_Note_Remaining",
                new System.Collections.Hashtable { { "@NoteId", noteId } });
            if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value) return 0m;
            return Convert.ToDecimal(dt.Rows[0][0]);
        }

        public decimal GetAccountBalance(int userId)
        {
            var dt = new DAL.Acceso().Leer("s_Account_Balance",
                new System.Collections.Hashtable { { "@UserId", userId } });
            if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value) return 0m;
            return Convert.ToDecimal(dt.Rows[0][0]);
        }

    }
}
