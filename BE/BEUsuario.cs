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
    }


    public class BEUsuarioAuth : BEUsuario
    {
        public string PasswordHash { get; set; } // formato "iter:saltB64:hashB64"
    }
}
