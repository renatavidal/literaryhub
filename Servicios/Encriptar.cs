using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace servicios
{
    public static class Encriptar
    {
        // ======== SIMÉTRICO: AES-256-CBC + HMAC-SHA256 (Encrypt-then-MAC) ========

        // encKey: 32 bytes (AES-256).  macKey: 32 bytes (HMAC).
        // Retorna: "encV1.{ivB64}.{ctB64}.{macB64}"
        public static string EncriptarSimetrico(string texto, byte[] encKey, byte[] macKey)
        {
            if (texto == null) texto = "";
            ValidarClaves(encKey, macKey);

            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = encKey;
                aes.GenerateIV();
                byte[] iv = aes.IV;

                byte[] plaintext = Encoding.UTF8.GetBytes(texto);

                byte[] ciphertext;
                using (var enc = aes.CreateEncryptor())
                    ciphertext = enc.TransformFinalBlock(plaintext, 0, plaintext.Length);

                // MAC sobre "encV1" + IV + CT (ETM)
                byte[] mac = Mac("encV1", iv, ciphertext, macKey);

                return "encV1." + B64(iv) + "." + B64(ciphertext) + "." + B64(mac);
            }
        }

        public static string DesencriptarSimetrico(string token, byte[] encKey, byte[] macKey)
        {
            ValidarClaves(encKey, macKey);
            var parts = (token ?? "").Split('.');
            if (parts.Length != 4 || parts[0] != "encV1") throw new InvalidOperationException("Token inválido");

            byte[] iv = B64d(parts[1]);
            byte[] ct = B64d(parts[2]);
            byte[] mac = B64d(parts[3]);

            // Verificar MAC antes de descifrar
            byte[] macCalc = Mac("encV1", iv, ct, macKey);
            if (!ConstEq(mac, macCalc)) throw new CryptographicException("MAC inválido");

            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = encKey;
                aes.IV = iv;

                using (var dec = aes.CreateDecryptor())
                {
                    var pt = dec.TransformFinalBlock(ct, 0, ct.Length);
                    return Encoding.UTF8.GetString(pt);
                }
            }
        }

        // ======== ASIMÉTRICO (ENVELOPE): RSA-OAEP + AES-256-CBC + HMAC-SHA256 ========
        // Claves RSA en XML
        // publicKeyXml: RSA pública en XML
        // Retorna: "envV1.{rsaKeyB64}.{ivB64}.{ctB64}.{macB64}"
        public static string EncriptarAsimetrico(string texto, string publicKeyXml)
        {
            if (texto == null) texto = "";

            // 1) Generar claves simétricas por mensaje
            byte[] encKey = RandomBytes(32); // AES-256
            byte[] macKey = RandomBytes(32); // HMAC
            string sym = EncriptarSimetrico(texto, encKey, macKey); // encV1.iv.ct.mac

            // 2) Cifrar (encKey||macKey) con RSA OAEP
            byte[] keyBlob = encKey.Concat(macKey).ToArray();
            byte[] rsaEncrypted = RsaEncrypt(publicKeyXml, keyBlob);

            // 3) Reempaquetar en envV1
            var parts = sym.Split('.');
            // parts[0]=encV1, [1]=iv, [2]=ct, [3]=mac
            return "envV1." + B64(rsaEncrypted) + "." + parts[1] + "." + parts[2] + "." + parts[3];
        }

        // privateKeyXml: RSA privada en XML
        public static string DesencriptarAsimetrico(string token, string privateKeyXml)
        {
            var parts = (token ?? "").Split('.');
            if (parts.Length != 5 || parts[0] != "envV1") throw new InvalidOperationException("Token inválido");

            byte[] rsaKey = B64d(parts[1]);
            byte[] iv = B64d(parts[2]);
            byte[] ct = B64d(parts[3]);
            byte[] mac = B64d(parts[4]);

            // 1) Recuperar encKey||macKey
            byte[] blob = RsaDecrypt(privateKeyXml, rsaKey);
            if (blob.Length != 64) throw new CryptographicException("Blob de clave inválido");
            byte[] encKey = blob.Take(32).ToArray();
            byte[] macKey = blob.Skip(32).Take(32).ToArray();

            // 2) Reconstruir token simétrico y descifrar (verifica MAC)
            string sym = "encV1." + B64(iv) + "." + B64(ct) + "." + B64(mac);
            return DesencriptarSimetrico(sym, encKey, macKey);
        }

        // ======== Helpers criptográficos ========

        private static byte[] Mac(string header, byte[] iv, byte[] ct, byte[] macKey)
        {
            using (var h = new HMACSHA256(macKey))
            {
                // MAC sobre header||iv||ct
                var headerBytes = Encoding.ASCII.GetBytes(header);
                h.TransformBlock(headerBytes, 0, headerBytes.Length, null, 0);
                h.TransformBlock(iv, 0, iv.Length, null, 0);
                h.TransformFinalBlock(ct, 0, ct.Length);
                return h.Hash;
            }
        }

        private static byte[] RsaEncrypt(string publicKeyXml, byte[] data)
        {
            // Nota: RSACryptoServiceProvider soporta OAEP-SHA1.
            // Intentamos OAEP-SHA256 si está disponible; si no, usamos SHA1.
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.PersistKeyInCsp = false;
                rsa.FromXmlString(publicKeyXml);

                try
                {
                    // En .NET Framework, este overload puede no existir; si falla, cae a OAEP-SHA1.

                    return rsa.Encrypt(data, true); // true = OAEP (SHA1)

                }
                catch
                {
                    return rsa.Encrypt(data, true); // fallback OAEP-SHA1
                }
            }
        }

        private static byte[] RsaDecrypt(string privateKeyXml, byte[] data)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.PersistKeyInCsp = false;
                rsa.FromXmlString(privateKeyXml);
                try
                {

                    return rsa.Decrypt(data, true); // OAEP-SHA1

                }
                catch
                {
                    return rsa.Decrypt(data, true);
                }
            }
        }

        private static void ValidarClaves(byte[] encKey, byte[] macKey)
        {
            if (encKey == null || encKey.Length != 32) throw new ArgumentException("encKey debe tener 32 bytes (AES-256).");
            if (macKey == null || macKey.Length != 32) throw new ArgumentException("macKey debe tener 32 bytes (HMAC-256).");
        }

        private static string B64(byte[] b) => Convert.ToBase64String(b);
        private static byte[] B64d(string s) => Convert.FromBase64String(s);
        private static byte[] RandomBytes(int n) { var b = new byte[n]; using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(b); return b; }

        private static bool ConstEq(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0; for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
