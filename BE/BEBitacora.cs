using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BEBitacora
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Descripcion { get; set; }
        public string Agente { get; set; }
        public DateTime FechaUtc { get; set; }
        public string Fecha => FechaUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
    }
    public class BEBitacoraFiltro
    {
        public int? UserId { get; set; }
        public DateTime? DesdeUtc { get; set; }
        public DateTime? HastaUtc { get; set; }
        public string Agente { get; set; }
        public string Texto { get; set; }
    }
}
