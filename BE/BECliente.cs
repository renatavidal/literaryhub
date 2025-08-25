using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BECliente
    {
        public int Id { get; set; }
        public string Cuil { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NegocioAlias { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Fac_CondIVA { get; set; }
        public string Fac_RazonSocial { get; set; }
        public string Fac_Cuit { get; set; }
        public string Fac_Domicilio { get; set; }
        public string Fac_Email { get; set; }
        public string Tipo { get; set; }       // 'AUT' | 'LIB'
        public string Ubicacion { get; set; }  // requerido si Tipo == 'LIB'

        public DateTime FechaAltaUtc { get; set; }
    }
}
