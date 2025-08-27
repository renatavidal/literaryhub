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
                {"@Ubicacion",       (object)c.Ubicacion ?? DBNull.Value},
                     {"@EmailActive", false}
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
        public BEUsuarioAuth GetUsuarioAuthByEmail(string email)
        {
            var h = new Hashtable { { "@Email", email } };
            var dt = _datos.Leer("s_cliente_por_email", h);
            if (dt.Rows.Count == 0) return null;

            var r = dt.Rows[0];
            var u = new BEUsuarioAuth
            {
                Id = Convert.ToInt32(r["Id"]),
                Email = Convert.ToString(r["Email"]),
                EmailVerified = Convert.ToBoolean(r["EmailActive"]),
                PasswordHash = Convert.ToString(r["PasswordHash"])
            };

            // Cargar roles
            u.Roles = GetRoles(u.Id);
            return u;
        }
        public string[] GetRoles(int userId)
        {
            var h = new Hashtable { { "@UserId", userId } };
            var dt = _datos.Leer("s_clientes_roles_listar", h);
            if (dt.Rows.Count == 0) return new string[0];

            var roles = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
                roles[i] = Convert.ToString(dt.Rows[i]["Rol"]);
            return roles;
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
        public void InsertEmailVerificationToken(int userId, string token, DateTime expiresAtUtc)
        {
            var h = new Hashtable
            {
                { "@UserId", userId },
                { "@Token",  token},
                { "@ExpiresAt", expiresAtUtc }

            };
            _datos.Escribir("sp_EmailToken_InsertClient", h);
        }
        public BEEmailVerificationToken GetToken(string token)
        {
            var h = new Hashtable
            {
                { "@Token", token }
            };

            DataTable dt = _datos.Leer("sp_EmailToken_GetClient", h);

            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            var be = new BEEmailVerificationToken
            {
                Id = (Guid)(row["Id"]),
                UserId = Convert.ToInt32(row["UserId"]),
                Token = Convert.ToString(row["Token"]),
                ExpiresAt = Convert.ToDateTime(row["ExpiresAt"]),
                Used = Convert.ToBoolean(row["Used"]),

            };

            return be;
        }
        public void MarkTokenUsed(Guid id)
        {
            var h = new Hashtable
            {
                { "@Id", id }
            };

            _datos.Escribir("sp_EmailToken_MarkUsedClient", h);
        }

        // Marcar email del usuario como verificado
        public void MarkEmailVerified(int userId)
        {
            var h = new Hashtable
            {
                { "@UserId", userId }
            };

            _datos.Escribir("sp_User_EmailVerified_SetClient", h);
        }
        public BEUsuarioAuth GetClienteAuthByEmail(string email)
        {
            var h = new Hashtable { { "@Email", email } };
            var dt = _datos.Leer("s_cliente_auth_por_email", h);
            if (dt.Rows.Count == 0) return null;

            var r = dt.Rows[0];
            return new BEUsuarioAuth
            {
                Id = Convert.ToInt32(r["Id"]),
                Email = Convert.ToString(r["Email"]),
                PasswordHash = Convert.ToString(r["PasswordHash"]),
                EmailVerified = Convert.ToBoolean(r["EmailVerified"]),
                Roles = new[] { "Reader", "Client" }    
            };
        }
       
    }
}
