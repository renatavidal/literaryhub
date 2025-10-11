using System;
using System.Data;
using MPP;

namespace BLL
{
    public class BLLFinanzasAdmin
    {
        private readonly MPPFinanzas _mpp = new MPPFinanzas();

        public DataTable NotasPorUsuario(int userId) => _mpp.ListarNotasPorUsuario(userId);
        public int CrearNotaCredito(int userId, decimal amount, string reason) => _mpp.CrearNota(userId, 'C', amount, reason);
        public int CrearNotaDebito(int userId, decimal amount, string reason) => _mpp.CrearNota(userId, 'D', amount, reason);
        public bool BorrarNota(int noteId) => _mpp.BorrarNota(noteId);

        public DataTable CuentaPorUsuario(int userId) => _mpp.ListarCuentaPorUsuario(userId);
        public bool AgregarMovimientoCC(int userId, decimal amount, string concept) => _mpp.AgregarMovimientoCC(userId, amount, concept);
        public bool BorrarMovimientoCC(int id) => _mpp.BorrarMovimientoCC(id);

        public decimal SaldoCuenta(int userId) => _mpp.GetAccountBalance(userId);
        public decimal SaldoNC(int noteId) => _mpp.GetCreditNoteRemaining(noteId);
        public (int creditNoteId, string number) RefundPurchase(int userId, int purchaseId, string reason = null)
        {
            if (userId <= 0) throw new ArgumentException("Usuario inválido.");
            if (purchaseId <= 0) throw new ArgumentException("Compra inválida.");
            return _mpp.RefundPurchase(userId, purchaseId, reason);
        }
    }
}
