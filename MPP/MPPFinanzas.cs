using System;
using System.Collections;
using System.Data;
using DAL;

namespace MPP
{
    public class MPPFinanzas
    {
        private readonly Acceso _db = new Acceso();

        public DataTable ListarNotasPorUsuario(int userId)
        {
            return _db.Leer("dbo.s_Notes_ByUser", new Hashtable { { "@UserId", userId } });
        }

        public int CrearNota(int userId, char type, decimal amount, string reason)
        {
            var hs = new Hashtable {
                { "@UserId", userId }, { "@Type", type }, { "@Amount", amount }, { "@Reason", reason }
            };
            // usp_Note_Create debe terminar con: SELECT SCOPE_IDENTITY()
            return _db.LeerCantidad("dbo.usp_Note_Create", hs);
        }

        public bool BorrarNota(int noteId)
        {
            return _db.Escribir("dbo.usp_Note_Delete", new Hashtable { { "@NoteId", noteId } });
        }
        public (int creditNoteId, string number) RefundPurchase(int userId, int purchaseId, string reason = null)
        {
            var h = new Hashtable {
                {"@PurchaseId", purchaseId},
                {"@UserId", userId},
                {"@Reason", (object)reason ?? DBNull.Value}
            };
            var dt = _db.Leer("usp_Purchase_Refund", h);
            int id = Convert.ToInt32(dt.Rows[0]["CreditNoteId"]);
            string num = Convert.ToString(dt.Rows[0]["Number"]);
            return (id, num);
        }

        public DataTable ListarCuentaPorUsuario(int userId)
        {
            return _db.Leer("dbo.s_AccountLedger_ByUser", new Hashtable { { "@UserId", userId } });
        }

        public bool AgregarMovimientoCC(int userId, decimal amount, string concept)
        {
            var hs = new Hashtable { { "@UserId", userId }, { "@Amount", amount }, { "@Concept", concept } };
            return _db.Escribir("dbo.usp_AccountLedger_Add", hs);
        }

        public bool BorrarMovimientoCC(int id)
        {
            return _db.Escribir("dbo.usp_AccountLedger_Delete", new Hashtable { { "@Id", id } });
        }

        // Aux: saldos para validaciones previas (ya los habías creado)
        public decimal GetCreditNoteRemaining(int noteId)
        {
            var dt = _db.Leer("dbo.s_Note_Remaining", new Hashtable { { "@NoteId", noteId } });
            if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value) return 0m;
            return Convert.ToDecimal(dt.Rows[0][0]);
        }
        public decimal GetAccountBalance(int userId)
        {
            var dt = _db.Leer("dbo.s_Account_Balance", new Hashtable { { "@UserId", userId } });
            if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value) return 0m;
            return Convert.ToDecimal(dt.Rows[0][0]);
        }
    }
}
