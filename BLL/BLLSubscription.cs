using System;
using System.Collections.Generic;
using Abstracciones;
using BE;
using MPP;

namespace BLL
{
    public class BLLSubscription
    {
        private readonly MPPSubscription _mpp = new MPPSubscription();
        private readonly IEmailService _mailer;   // ya lo tenés

        public BLLSubscription(IEmailService mailer) { _mailer = mailer; }
        public BLLSubscription() { }
        public decimal GetCreditNoteRemainingByUser(int noteId, int userId)
        {
            return _mpp.GetCreditNoteRemainingByUser(noteId, userId);
        }
        public BEActiveSubscription GetActiveForUser(int userId) { 
          return _mpp.GetActiveForUser(userId);
        }
        public void CancelForUser(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Usuario inválido.");
            _mpp.CancelForUser(userId);
        }

        public int Checkout(int userId, string planCode, string currency, List<BEPaymentSplit> splits, string userEmail)
        {
            // 1) Crear orden
            int orderId = _mpp.BeginOrder(userId, planCode, currency);

            // 2) Validaciones de negocio
            if (splits == null || splits.Count == 0) throw new InvalidOperationException("Debe informar al menos un pago.");
            decimal total = 0m; foreach (var s in splits) { if (s.Amount <= 0) throw new InvalidOperationException("Importe de pago inválido."); total += s.Amount; }

            // (opcional) consultar el total esperado de la orden si querés verificar aquí
            // y/o traer precio del plan en BE y comparar "total == precio"

            // 3) Registrar cada pago (tarjeta / NC / cta cte)
            foreach (var s in splits)
                _mpp.RegisterPayment(orderId, s);

            // 4) Confirmar: marca pagada, genera factura y actualiza roles
            _mpp.ConfirmOrder(orderId);

            // 5) Aviso por mail
            try
            {
                var appName = "LiteraryHub";
                var footer = "© " + appName + ". Todos los derechos reservados.";

                var tokens = new Dictionary<string, string>();
                tokens["AppName"] = appName;
                tokens["Title"] = "¡Gracias por tu suscripción!";
                tokens["Preheader"] = $"<p>Tu orden <strong>#{orderId}</strong> fue acreditada.</p>" +
                     $"<p>Ya actualizamos tus permisos según el plan <strong>{planCode}</strong>.</p>";
                tokens["Tagline"] = "Lectura, comunidad y libros";
                tokens["IntroHtml"] = $"Tu orden #{orderId} fue acreditada. " +
                               $"Ya actualizamos tus permisos según el plan {planCode}.";

                tokens["SupportEmail"] = "soporte@literaryhub.com";
                tokens["FooterText"] = "© LiteraryHub. Todos los derechos reservados.";
                tokens["CompanyName"] = "LiteraryHub";
                tokens["CompanyAddress"] = "Buenos Aires, AR";

                string templatePath = System.Web.Hosting.HostingEnvironment.MapPath("/Templates/Email/BaseTemplate.html");
                _mailer.SendTemplate(userEmail, "Verificación de email", templatePath, tokens);
            }
            catch { /* no hago fallar la compra por el mail */ }

            return orderId;
        }

        public BE.BEPlan GetPlanByCode(string code)
        {
            return _mpp.GetPlanByCode(code);
        }
        public decimal GetCreditNoteRemaining(int noteId){
            return _mpp.GetCreditNoteRemaining(noteId);
        } 
        public decimal GetAccountBalance(int userId){ return _mpp.GetAccountBalance(userId); }

    }
}
