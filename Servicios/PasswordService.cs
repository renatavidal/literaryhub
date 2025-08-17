// Servicios/PasswordService.cs
using System;
using System.Security.Cryptography;
using System.Text;

namespace Servicios
{
    public static class PasswordService
    {
        private const int Iterations = 150_000;
        private const int SaltSize = 16;   // 128-bit
        private const int KeySize = 32;   // 256-bit

        public static string HashPassword(string password)
        {
            if (password == null) password = "";
            // Sal aleatoria por usuario
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // En net472 este ctor usa HMAC-SHA1 internamente
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                var key = pbkdf2.GetBytes(KeySize);
                return Iterations.ToString() + ":" +
                       Convert.ToBase64String(salt) + ":" +
                       Convert.ToBase64String(key);
            }

          
        }

        public static bool VerifyPassword(string password, string stored)
        {
            if (string.IsNullOrWhiteSpace(stored)) return false;
            var parts = stored.Split(':');
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0], out var iter)) return false;

            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);


            using (var pbkdf2 = new Rfc2898DeriveBytes(password ?? string.Empty, salt, iter))
            {
                var key = pbkdf2.GetBytes(hash.Length);
                return ConstantTimeEquals(hash, key);
            }
        }
        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= (a[i] ^ b[i]);
            return diff == 0;
        }
    }
}
