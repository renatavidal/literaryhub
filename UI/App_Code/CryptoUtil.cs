using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class CryptoUtil
{
    // Clave en Base64 (32 bytes => AES-256)
    private static byte[] Key
    {
        get
        {
            var b64 = ConfigurationManager.AppSettings["CardAesKeyB64"];
            if (string.IsNullOrEmpty(b64)) throw new InvalidOperationException("CardAesKeyB64 no configurado.");
            return Convert.FromBase64String(b64);
        }
    }

    public static void EncryptPan(string pan, out byte[] cipher, out byte[] iv)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = Key;
            aes.GenerateIV();
            iv = aes.IV;
            using (var enc = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8))
            {
                sw.Write(pan ?? "");
                sw.Flush(); cs.FlushFinalBlock();
                cipher = ms.ToArray();
            }
        }
    }

    public static string DecryptPan(byte[] cipher, byte[] iv)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = Key; aes.IV = iv;
            using (var dec = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(cipher))
            using (var cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
