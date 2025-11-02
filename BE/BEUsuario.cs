using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// BE/BEUsuario.cs
namespace BE
{
    public class BEUsuario
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string[] Roles { get; set; } = new string[0];
        public bool Activo { get; set; }
    }

    public class BEIdTexto
    {
        public int Id { get; set; }
        public string Texto { get; set; }
    }
    public class BEUsuarioAuth : BEUsuario
    {
        public string PasswordHash { get; set; } // formato "iter:saltB64:hashB64"
    }
    public class BENombre: BEUsuario
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
    }
    public class BEUsuarioMini
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
    }
}
