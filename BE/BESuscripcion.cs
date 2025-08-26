using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BESuscripcion
    {
        public int Id { get; set; }
        public string Codigo { get; set; }       
        public string Descripcion { get; set; }   
        public string Roles { get; set; }         
        public decimal PrecioUSD { get; set; }
        public int Orden { get; set; }
        public bool EsDestacado { get; set; }
        public bool Activo { get; set; }
    }
}
