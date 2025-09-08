using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public enum PersonaClase { Usuario, Cliente }
    public class BEPersona
    {
        public PersonaClase Clase { get; set; }      // Usuario | Cliente
        public int Id { get; set; }                  // PK de Usuarios o Clientes
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string[] Roles { get; set; } = Array.Empty<string>();
        public bool Activo { get; set; }
    }
}
