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
    public class BLLUsuario: BLLPersona
    {
        private readonly MPPUsuario _mpp;
        private readonly IEmailService _email;

        public BLLUsuario()
        {
            _mpp = new MPPUsuario();
            _email = new SmtpEmailService();
        }

        public int Registrar(string email, string nombre, string apellido, string passwordPlano, bool emailVerifiedInicial = false)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email requerido.");
            if (string.IsNullOrEmpty(passwordPlano)) throw new ArgumentException("Contraseña requerida.");
            var ph = PasswordService.HashPassword(passwordPlano);

            int nuevoId = _mpp.CrearUsuario(email, nombre, apellido, ph, emailVerifiedInicial);
            return nuevoId; 
        }
        public void RegistrarRegistro(BEUsuario u, string agente)
        {
            if (u == null) return;
            _mpp.RegistrarRegistro(u.Id, agente);
        }
        public void Deactivate(int userId)
        {
            _mpp.Deactivate(userId);
            RegistrarEnBitacora(userId, "", "Usuario dado de Baja");
        }
        public void RegistrarEnBitacora(int id, string agente, string accion)
        {
            _mpp.RegistrarEnBitacora(id, agente, accion);
        }

        public void RegistrarAcceso(BEPersona u,  string agente)
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
        public BEPersona LoginAny(string email, string passwordPlano)
        {
            var u = _mpp.GetUsuarioAuthByEmail(email);
            if (u != null && PasswordService.VerifyPassword(passwordPlano, u.PasswordHash))
                return new BEPersona
                {
                    Clase = PersonaClase.Usuario,
                    Id = u.Id,
                    Email = u.Email,
                    EmailVerified = u.EmailVerified,
                    Roles = u.Roles ?? Array.Empty<string>(),
                    Activo = u.Activo,
                };
            MPPCliente _mppCliente = new MPPCliente();
            var c = _mppCliente.GetClienteAuthByEmail(email);
            if (c != null && PasswordService.VerifyPassword(passwordPlano, c.PasswordHash))
                return new BEPersona
                {
                    Clase = PersonaClase.Cliente,
                    Id = c.Id,
                    Email = c.Email,
                    EmailVerified = c.EmailVerified,
                    Roles = c.Roles ?? new[] { "Reader", "Client" },
                    Activo = c.Activo,
                };

            return null;
        }

        public BEUsuario Login(string email, string passwordPlano)
        {
            var rec = _mpp.GetUsuarioAuthByEmail(email);
            if (rec == null) return null;

            bool ok = PasswordService.VerifyPassword(passwordPlano, rec.PasswordHash);
            if (!ok) return null;



            return new BEUsuario
            {
                Id = rec.Id,
                Email = rec.Email,
                EmailVerified = rec.EmailVerified,
                Roles = rec.Roles ?? new string[0],
                Activo = rec.Activo,
            };
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
        public List<BESuscripcion> ListarPlanesSuscripcion()
        {
            return _mpp.GetPlanesSuscripcion();
        }
        public List<BESuscripcion> ListarPlanesAdmin()
        {
            return _mpp.GetPlanesSuscripcionAdmin();
        }
        public int CrearPlan(BESuscripcion s)
        {
            ValidarPlan(s, isNew: true);
            return _mpp.InsertPlan(s);
        }

        public void ActualizarPlan(BESuscripcion s)
        {
            if (s.Id <= 0) throw new ArgumentException("Id inválido.");
            ValidarPlan(s, isNew: false);
            _mpp.UpdatePlan(s);
        }

        public void ArchivarPlan(int id)
        {
            if (id <= 0) throw new ArgumentException("Id inválido.");
            _mpp.ArchivarPlan(id);
        }

        private void ValidarPlan(BESuscripcion s, bool isNew)
        {
            if (string.IsNullOrWhiteSpace(s.Codigo) || s.Codigo.Length > 30)
                throw new ArgumentException("Código requerido (máx. 30).");
            if (string.IsNullOrWhiteSpace(s.Descripcion) || s.Descripcion.Length > 120)
                throw new ArgumentException("Descripción requerida (máx. 120).");
            if (string.IsNullOrWhiteSpace(s.Roles) || s.Roles.Length > 200)
                throw new ArgumentException("Roles requeridos (CSV, máx. 200).");
            if (s.PrecioUSD < 0) throw new ArgumentException("Precio no puede ser negativo.");
            if (s.Orden < 0) s.Orden = 0;
        }
        
        public List<BEIdTexto> ListarUsuariosParaFiltro()
        {
            return _mpp.ListarUsuariosParaFiltro();
        }
        public  List<BENombre> Buscar(string texto)
        {
            return _mpp.Buscar(texto);
        }

        public  bool DarDeBaja(int userId, string agente)
        {
            bool ok = _mpp.SetActive(userId, false);
            try { RegistrarEnBitacora( userId, agente, "Admin: BAJA de usuario"); } catch { }
            return ok;
        }

        public  bool DarDeAlta(int userId, string agente)
        {
            bool ok = _mpp.SetActive(userId, true);
            try { RegistrarEnBitacora( userId , agente, "Admin: ALTA de usuario"); } catch { }
            return ok;
        }
    }


}


