using System;
using System.Linq;
using BE;

namespace BLL
{
    public class BLLCliente: BLLPersona
    {
        private readonly MPP.MPPCliente _mpp = new MPP.MPPCliente();
        private readonly BLLBitacora _bit = new BLLBitacora(); 

        public int Registrar(BECliente c)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(c.Cuil) || !ValidarCuitCuil(c.Cuil))
                throw new ArgumentException("CUIL inválido.");

            if (_mpp.ExisteCuil(c.Cuil) > 0)
                throw new InvalidOperationException("El CUIL ya está registrado.");

            if (_mpp.ExisteEmail(c.Email) > 0)
                throw new InvalidOperationException("El email ya está registrado.");
            c.Tipo = (c.Tipo ?? "AUT").ToUpperInvariant();
            if (c.Tipo != "AUT" && c.Tipo != "LIB")
                throw new ArgumentException("Tipo inválido. Debe ser AUT (Autor) o LIB (Librería).");
            if (c.Tipo == "LIB" && string.IsNullOrWhiteSpace(c.Ubicacion))
                throw new ArgumentException("La librería debe informar una ubicación.");

            // Si no informaron facturación, usar datos del titular
            if (string.IsNullOrEmpty(c.Fac_Cuit)) c.Fac_Cuit = c.Cuil;
            if (string.IsNullOrEmpty(c.Fac_RazonSocial)) c.Fac_RazonSocial = (c.Nombre + " " + (c.Apellido ?? "")).Trim();

            int id = _mpp.Insertar(c);

            // Bitácora
            try { _bit.Registrar(null, $"Alta de cliente #{id} ({c.Email})", "Cliente/Signup"); } catch { }

            return id;
        }

        public static bool ValidarCuitCuil(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return false;
            var s = new string(valor.Where(char.IsDigit).ToArray());
            if (s.Length != 11) return false;

            int[] mult = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int sum = 0; for (int i = 0; i < 10; i++) sum += (s[i] - '0') * mult[i];
            int mod = 11 - (sum % 11);
            int dig = (mod == 11) ? 0 : (mod == 10) ? 9 : mod;
            return dig == (s[10] - '0');
        }

        public System.Collections.Generic.List<BECliente> Buscar(string texto)
        {
            return _mpp.Buscar(texto);
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
    }
}
