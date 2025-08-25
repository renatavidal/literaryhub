using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DAL;
using BE;

namespace MPP
{
    public class MPPCliente
    {
        private readonly Acceso _datos = new Acceso(); // tu DAL

        public int Insertar(BE.BECliente c)
        {
                 var h = new Hashtable {
                {"@Cuil",            c.Cuil},
                {"@Nombre",          c.Nombre},
                {"@Apellido",        (object)c.Apellido ?? DBNull.Value},
                {"@NegocioAlias",    (object)c.NegocioAlias ?? DBNull.Value},
                {"@Email",           c.Email},
                {"@Telefono",        (object)c.Telefono ?? DBNull.Value},
                {"@Fac_CondIVA",     (object)c.Fac_CondIVA ?? DBNull.Value},
                {"@Fac_RazonSocial", (object)c.Fac_RazonSocial ?? DBNull.Value},
                {"@Fac_Cuit",        (object)c.Fac_Cuit ?? DBNull.Value},
                {"@Fac_Domicilio",   (object)c.Fac_Domicilio ?? DBNull.Value},
                {"@Fac_Email",       (object)c.Fac_Email ?? DBNull.Value},
                {"@Tipo",            c.Tipo},
                {"@Ubicacion",       (object)c.Ubicacion ?? DBNull.Value}
            };

            return _datos.LeerCantidad("sp_Cliente_Insert", h);
        }


        public int ExisteEmail(string email)
        {
            var h = new Hashtable { { "@Email", email } };
            return _datos.LeerCantidad("sp_Cliente_ExisteEmail", h);
        }

        public int ExisteCuil(string cuil)
        {
            var h = new Hashtable { { "@Cuil", cuil } };
            return _datos.LeerCantidad("sp_Cliente_ExisteCuil", h);
        }

        public List<BE.BECliente> Buscar(string texto)
        {
            var h = new Hashtable { { "@Texto", (object)texto ?? DBNull.Value } };
            var dt = _datos.Leer("sp_Cliente_Buscar", h);

            var list = new List<BE.BECliente>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new BE.BECliente
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Cuil = Convert.ToString(r["Cuil"]),
                    Nombre = Convert.ToString(r["Nombre"]),
                    Apellido = r["Apellido"] == DBNull.Value ? null : Convert.ToString(r["Apellido"]),
                    NegocioAlias = r["NegocioAlias"] == DBNull.Value ? null : Convert.ToString(r["NegocioAlias"]),
                    Email = Convert.ToString(r["Email"]),
                    Tipo = r["Tipo"] == DBNull.Value ? "" :  Convert.ToString(r["Tipo"]),
                    Ubicacion = r["Tipo"] == "LIB"? (r["Ubicacion"] == DBNull.Value ? null : Convert.ToString(r["Ubicacion"])): null,
                    Telefono = r["Telefono"] == DBNull.Value ? null : Convert.ToString(r["Telefono"]),
                    Fac_CondIVA = r["Fac_CondIVA"] == DBNull.Value ? null : Convert.ToString(r["Fac_CondIVA"]),
                    Fac_RazonSocial = r["Fac_RazonSocial"] == DBNull.Value ? null : Convert.ToString(r["Fac_RazonSocial"]),
                    Fac_Cuit = r["Fac_Cuit"] == DBNull.Value ? null : Convert.ToString(r["Fac_Cuit"]),
                    Fac_Domicilio = r["Fac_Domicilio"] == DBNull.Value ? null : Convert.ToString(r["Fac_Domicilio"]),
                    Fac_Email = r["Fac_Email"] == DBNull.Value ? null : Convert.ToString(r["Fac_Email"]),
                    FechaAltaUtc = Convert.ToDateTime(r["FechaAltaUtc"])
                });
            }
            return list;
        }
    }
}
