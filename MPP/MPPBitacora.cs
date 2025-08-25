using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DAL;

namespace MPP
{
    public class MPPBitacora
    {
        private readonly Acceso _datos = new Acceso(); 

        public void Insertar(int? userId, string descripcion, string agente)
        {
            var h = new Hashtable {
                {"@UserId", (object)userId ?? DBNull.Value},
                {"@Descripcion", descripcion},
                {"@Agente", (object)agente ?? DBNull.Value}
            };
            _datos.Escribir("sp_Bitacora_Insert", h);
        }

        public List<BE.BEBitacora> Buscar(BE.BEBitacoraFiltro f)
        {
            var h = new Hashtable {
                {"@UserId",   (object)f.UserId   ?? DBNull.Value},
                {"@DesdeUtc", (object)f.DesdeUtc ?? DBNull.Value},
                {"@HastaUtc", (object)f.HastaUtc ?? DBNull.Value},
                {"@Agente",   (object)f.Agente   ?? DBNull.Value},
                {"@Texto",    (object)f.Texto    ?? DBNull.Value}
            };

            var dt = _datos.Leer("sp_Bitacora_Buscar", h);

            var list = new List<BE.BEBitacora>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new BE.BEBitacora
                {
                    Id = Convert.ToInt32(r["Id"]),
                    UserId = r.IsNull("UserId") ? 0 : Convert.ToInt32(r["UserId"]),
                    Descripcion = Convert.ToString(r["Descripcion"]),
                    Agente = r["Agente"] == DBNull.Value ? null : Convert.ToString(r["Agente"]),
                    FechaUtc = Convert.ToDateTime(r["FechaUtc"])
                });
            }
            return list;
        }
    }
}
