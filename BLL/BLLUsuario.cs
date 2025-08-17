using BE;
using MPP;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLUsuario
    {
        private readonly MPPUsuario _mpp;

        public BLLUsuario()
        {
            _mpp = new MPPUsuario();
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
        public int Registrar(string email, string passwordPlano, bool emailVerifiedInicial = false)
        {
            // Validaciones mínimas
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email requerido.");
            if (string.IsNullOrEmpty(passwordPlano)) throw new ArgumentException("Contraseña requerida.");

            // Hash PBKDF2 (formato "iter:salt:hash")
            var ph = PasswordService.HashPassword(passwordPlano);

            // Crear en DB
            int nuevoId = _mpp.CrearUsuario(email, ph, emailVerifiedInicial);
            return nuevoId; // 0 o -1 si tu SP no retorna identity
        }

        public void RegistrarAcceso(BEUsuario u,  string agente)
        {
            if (u == null) return;
            _mpp.RegistrarAcceso(u.Id,  agente);
        }
    }
}

