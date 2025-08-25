using BE;
using MPP;
using Servicios;
using System;
using System.Web;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abstracciones;
using Servicios;
using static System.Net.Mime.MediaTypeNames;

namespace BLL
{
    public class BLLUsuario
    {
        private readonly MPPUsuario _mpp;
        private readonly IEmailService _email;

        public BLLUsuario()
        {
            _mpp = new MPPUsuario();
            _email = new SmtpEmailService();
        }


        /// <summary>
        /// Valida credenciales. Devuelve BEUsuario (sin exponer PasswordHash) o null si inválidas.
        /// </summary>
        public BEUsuario Login(string email, string passwordPlano)
        {
            var rec = _mpp.GetUsuarioAuthByEmail(email);
            if (rec == null) return null;

            // Verificación PBKDF2
            bool ok = PasswordService.VerifyPassword(passwordPlano, rec.PasswordHash);
            if (!ok) return null;

  

            return new BEUsuario
            {
                Id = rec.Id,
                Email = rec.Email,
                EmailVerified = rec.EmailVerified,
                Roles = rec.Roles ?? new string[0]
            };
        }

        /// <summary>
        /// Registro de usuario: hashea contraseña y crea en DB. Devuelve el Id creado.
        /// </summary>
        public int Registrar(string email, string nombre, string apellido, string passwordPlano, bool emailVerifiedInicial = false)
        {
            // Validaciones mínimas
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email requerido.");
            if (string.IsNullOrEmpty(passwordPlano)) throw new ArgumentException("Contraseña requerida.");

            // Hash PBKDF2 (formato "iter:salt:hash")
            var ph = PasswordService.HashPassword(passwordPlano);

            // Crear en DB
            int nuevoId = _mpp.CrearUsuario(email, nombre, apellido, ph, emailVerifiedInicial);
            return nuevoId; // 0 o -1 si tu SP no retorna identity
        }
        public void RegistrarRegistro(BEUsuario u, string agente)
        {
            if (u == null) return;
            _mpp.RegistrarRegistro(u.Id, agente);
        }

        public void RegistrarAcceso(BEUsuario u,  string agente)
        {
            if (u == null) return;
            _mpp.RegistrarAcceso(u.Id,  agente);
        }
        public void RegistrarCambioContrasena(BEUsuario u, string agente)
        {
            if (u == null) return;
            _mpp.RegistrarCambioContrasena(u.Id, agente);
        }
        public void EnviarVerificacion(string toEmail, string verifyUrl)
        {
            var appName = "LiteraryHub";
            var footer = "© " + appName + ". Todos los derechos reservados.";

            var tokens = new Dictionary<string, string>();
            tokens["AppName"] = appName;
            tokens["Title"] = "Confirmá tu email";
            tokens["Preheader"] = "Activá tu cuenta con un clic.";
            tokens["Tagline"] = "Lectura, comunidad y libros";
            tokens["IntroHtml"]  = "¡Gracias por registrarte! Para activar tu cuenta, hacé clic en el botón de abajo.";
            tokens["ButtonText"] = "Verificar mi email";
            tokens["ButtonUrl"] = verifyUrl;
            tokens["SupportEmail"] = "soporte@literaryhub.com";
            tokens["FooterText"] = "© LiteraryHub. Todos los derechos reservados.";
            tokens["CompanyName"] = "LiteraryHub";
            tokens["CompanyAddress"] = "Buenos Aires, AR";

            string templatePath = System.Web.Hosting.HostingEnvironment.MapPath("/Templates/Email/BaseTemplate.html");
            _email.SendTemplate(toEmail, "Verificación de email", templatePath, tokens);
        }
        public void EnviarVerificacionContrasena(string toEmail, string verifyUrl)
        {
            var appName = "LiteraryHub";
            var footer = "© " + appName + ". Todos los derechos reservados.";

            var tokens = new Dictionary<string, string>();
            tokens["AppName"] = appName;
            tokens["Title"] = "Cambia tu contraseña";
            tokens["Preheader"] = "Cambia tu contraseña con un clic.";
            tokens["Tagline"] = "Lectura, comunidad y libros";
            tokens["IntroHtml"] = " Para cambiar tu contraseña, hacé clic en el botón de abajo.";
            tokens["ButtonText"] = "cambiar constaseña";
            tokens["ButtonUrl"] = verifyUrl;
            tokens["SupportEmail"] = "soporte@literaryhub.com";
            tokens["FooterText"] = "© LiteraryHub. Todos los derechos reservados.";
            tokens["CompanyName"] = "LiteraryHub";
            tokens["CompanyAddress"] = "Buenos Aires, AR";

            string templatePath = System.Web.Hosting.HostingEnvironment.MapPath("/Templates/Email/BaseTemplate.html");
            _email.SendTemplate(toEmail, "Cambio de Contraseña", templatePath, tokens);
        }
        public string GenerarTokenVerificacion(int userId)
        {

            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                          .Replace("+", string.Empty)
                          .Replace("/", string.Empty)
                          .TrimEnd('=');
            GuardarToken(userId, token, DateTime.Now.AddHours(20));
            return token;
        }
        public void GuardarToken(int userId, string token, DateTime expiresAt)
        {
            _mpp.InsertEmailVerificationToken(userId, token, expiresAt);
        }
        public bool VerificarEmailPorToken(string token, out int userId)
        {
            userId = 0;
            var t = _mpp.GetToken(token);
            if (t == null) return false;
            if (t.Used) return false;
            if (t.ExpiresAt < DateTime.UtcNow) return false;

            _mpp.MarkEmailVerified(t.UserId);
            _mpp.MarkTokenUsed(t.Id);
            userId = t.UserId;
            return true;
        }
        public BEUsuario ObtenerPorEmail(string email)
        {
            return _mpp.GetUserByEmail(email);
        }

        public void CambiarPassword(int userId, string newPassword)
        {
            string hash = PasswordService.HashPassword(newPassword);
            _mpp.RegistrarCambioContrasena(userId, "");
            _mpp.SetPassword(userId, hash);
        }
        public void Contacto(string email, string text, int userId)
        {
            _mpp.Contacto(email, text, userId);
        }
        public bool ExisteEmail(string email) {
            return _mpp.ExisteEmail( email);
        }


    }
}

