// MPP/MPPUsuario.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;
using DAL;
using static System.Net.Mime.MediaTypeNames;

namespace MPP
{
    public class MPPUsuario
    {
        private readonly Acceso _datos;

        // Opción recomendada: recibir CS por el constructor y crear DAL.
        public MPPUsuario()
        {
            _datos = new Acceso();
        }

        // Si tu DAL tiene ctor sin parámetros que ya toma la CS internamente,
        // podés habilitar este constructor:
        public MPPUsuario(Acceso acceso)
        {
            _datos = acceso ?? throw new ArgumentNullException(nameof(acceso));
        }
        public DataTable ByUser(int userId)
        {
            var h = new Hashtable { { "@UserId", userId } };
            return _datos.Leer("usp_Purchases_ByUser", h);
        }
        public BEUsuarioAuth GetUsuarioAuthByEmail(string email)
        {
            var h = new Hashtable { { "@Email", email } };
            var dt = _datos.Leer("s_usuario_por_email", h);
            if (dt.Rows.Count == 0) return null;

            var r = dt.Rows[0];
            var u = new BEUsuarioAuth
            {
                Id = Convert.ToInt32(r["Id"]),
                Email = Convert.ToString(r["Email"]),
                EmailVerified = Convert.ToBoolean(r["EmailVerified"]),
                PasswordHash = Convert.ToString(r["PasswordHash"]),
                Activo = Convert.ToBoolean(r["Activo"])
            };

            // Cargar roles
            u.Roles = GetRoles(u.Id);
            return u;
        }
        public void RegistrarEnBitacora(int id, string agente, string accion)
        {
            var h = new Hashtable { { "@UserId",id }, { "@Agente", agente }, {"@Descripcion", accion } };
            _datos.Escribir("s_registrar_en_bitacora", h);
        }
        public void Deactivate(int userId)
        {
            var h = new Hashtable { { "@UserId", userId } };
            _datos.LeerCantidad("sp_User_Deactivate", h); 
        }

        public string[] GetRoles(int userId)
        {
            var h = new Hashtable { { "@UserId", userId } };
            var dt = _datos.Leer("s_usuario_roles_listar", h);
            if (dt.Rows.Count == 0) return new string[0];

            var roles = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
                roles[i] = Convert.ToString(dt.Rows[i]["Rol"]);
            return roles;
        }

        /// <summary>Crea usuario y devuelve el nuevo Id. Requiere que el SP haga SELECT SCOPE_IDENTITY().</summary>
        public int CrearUsuario(string email, string nombre, string apellido, string passwordHash, bool emailVerified)
        {
            var h = new Hashtable
            {   
                { "@Email", email },
                { "@Nombre", nombre },
                { "@Apellido", apellido },
                { "@PasswordHash", passwordHash },
                { "@EmailVerified", emailVerified }
            };
            // s_usuario_crear debe hacer: INSERT ...; SELECT CAST(SCOPE_IDENTITY() AS INT);
            return _datos.LeerCantidad("s_usuario_crear", h);
        }

        /// <summary>Actualiza el PasswordHash (para rehash/upgrade).</summary>
        public bool ActualizarPasswordHash(int userId, string nuevoHash)
        {
            var h = new Hashtable { { "@UserId", userId }, { "@PasswordHash", nuevoHash } };
            // SP: s_usuario_actualizar_hash @UserId, @PasswordHash
            return _datos.Escribir("s_usuario_actualizar_hash", h);
        }

        /// <summary>Registra bitácora de acceso.</summary>
        public void RegistrarAcceso(int userId, string agente)
        {
            var h = new Hashtable
            {
                { "@UserId", userId },
                { "@Descripcion",  "Usuario logueado" },
                { "@Agente", agente ?? "" },
                { "@Fecha", DateTime.UtcNow }
            };
            _datos.Escribir("s_bitacora_login", h);
        }
        public void RegistrarCambioContrasena(int userId, string agente)
        {
            var h = new Hashtable
            {
                { "@UserId", userId },
                { "@Descripcion",  "Usuario modifico su contraseña" },
                { "@Agente", agente ?? "" },
                { "@Fecha", DateTime.UtcNow }
            };
            _datos.Escribir("s_bitacora_login", h);
        }
        public void RegistrarRegistro(int userId, string agente)
        {
            var h = new Hashtable
            {
                { "@UserId", userId },
                { "@Descripcion",  "Nuevo Usuario Registrado" },
                { "@Agente", agente ?? "" },
                { "@Fecha", DateTime.UtcNow }
            };
            _datos.Escribir("s_bitacora_login", h);
        }
        public void InsertEmailVerificationToken(int userId, string token, DateTime expiresAtUtc)
        {
            var h = new Hashtable
            {
                { "@UserId", userId },
                { "@Token",  token},
                { "@ExpiresAt", expiresAtUtc }

            };
             _datos.Escribir("sp_EmailToken_Insert", h);
        }
        public BEEmailVerificationToken GetToken(string token)
        {
            var h = new Hashtable
            {
                { "@Token", token }
            };

            DataTable dt = _datos.Leer("sp_EmailToken_Get", h);

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

            _datos.Escribir("sp_EmailToken_MarkUsed", h);
        }

        // Marcar email del usuario como verificado
        public void MarkEmailVerified(int userId)
        {
            var h = new Hashtable
            {
                { "@UserId", userId }
            };

            _datos.Escribir("sp_User_EmailVerified_Set", h);
        }
        public BEUsuarioAuth GetUserByEmail(string email)
        {
            var h = new Hashtable { { "@Email", email } };
            DataTable dt = _datos.Leer("sp_User_GetByEmail", h);
            if (dt.Rows.Count == 0) return null;

            DataRow r = dt.Rows[0];
            var be = new BEUsuarioAuth();
            be.Id = Convert.ToInt32(r["Id"]);
            be.Email = Convert.ToString(r["Email"]);
            be.PasswordHash = Convert.ToString(r["PasswordHash"]);
            be.EmailVerified = Convert.ToBoolean(r["EmailVerified"]);
            return be;
        }
        public void SetPassword(int userId, string passwordHash)
        {
            var h = new Hashtable {
            { "@UserId", userId },
            { "@PasswordHash", passwordHash }
        };
            _datos.Escribir("sp_User_SetPassword", h);
        }
        public void MarkTokenUsed(int tokenId)
        {
            var h = new Hashtable { { "@Id", tokenId } };
            _datos.Escribir("sp_EmailToken_MarkUsed", h);
        }
        public void Contacto(string email, string text, int userId)
        {
            var h = new Hashtable { { "@UserId ", userId } ,
                {"@Email", email }, {"@Text", text} };
            _datos.Escribir("sp_Contacto", h);
        }
        public bool ExisteEmail(string email)
        {
            var h = new Hashtable { 
                {"@Email", email } };
            if (_datos.LeerCantidad("s_email_existente", h) > 0)
                return true;
            return false;
        }
        public List<BE.BESuscripcion> GetPlanesSuscripcionAdmin()
        {
            var dt = _datos.Leer("sp_Suscripciones_ListarTodo", null);
            var list = new List<BE.BESuscripcion>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new BE.BESuscripcion
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Codigo = Convert.ToString(r["Codigo"]),
                    Descripcion = Convert.ToString(r["Descripcion"]),
                    Roles = Convert.ToString(r["Roles"]),
                    PrecioUSD = Convert.ToDecimal(r["PrecioUSD"]),
                    Orden = Convert.ToInt32(r["Orden"]),
                    EsDestacado = Convert.ToBoolean(r["EsDestacado"]),
                    Activo = Convert.ToBoolean(r["Activo"])
                });
            }
            return list;
        }
        public List<BE.BESuscripcion> GetPlanesSuscripcion()
        {
            var dt = _datos.Leer("sp_Suscripciones_ListarActivas", null);
            var list = new List<BE.BESuscripcion>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new BE.BESuscripcion
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Codigo = Convert.ToString(r["Codigo"]),
                    Descripcion = Convert.ToString(r["Descripcion"]),
                    Roles = Convert.ToString(r["Roles"]),
                    PrecioUSD = Convert.ToDecimal(r["PrecioUSD"]),
                    Orden = Convert.ToInt32(r["Orden"]),
                    EsDestacado = Convert.ToBoolean(r["EsDestacado"])
                });
            }
            return list;
        }

        public int InsertPlan(BE.BESuscripcion s)
        {
            var h = new Hashtable {
                {"@Codigo", s.Codigo}, {"@Descripcion", s.Descripcion}, {"@Roles", s.Roles},
                {"@PrecioUSD", s.PrecioUSD}, {"@Orden", s.Orden},
                {"@EsDestacado", s.EsDestacado}, {"@Activo", s.Activo}
            };
            return _datos.LeerCantidad("sp_Suscripciones_Insert", h);
        }

        public void UpdatePlan(BE.BESuscripcion s)
        {
            var h = new Hashtable {
                {"@Id", s.Id}, {"@Codigo", s.Codigo}, {"@Descripcion", s.Descripcion},
                {"@Roles", s.Roles}, {"@PrecioUSD", s.PrecioUSD}, {"@Orden", s.Orden},
                {"@EsDestacado", s.EsDestacado}, {"@Activo", s.Activo}
            };
            _datos.Escribir("sp_Suscripciones_Update", h);
        }

        public void ArchivarPlan(int id)
        {
            var h = new Hashtable { { "@Id", id } };
            _datos.Escribir("sp_Suscripciones_Archivar", h);
        }
        public List<BEIdTexto> ListarUsuariosParaFiltro()
        {
            var dt = _datos.Leer("s_usuarios_listar_para_filtro", null);
            var list = new List<BEIdTexto>();
            foreach (System.Data.DataRow r in dt.Rows)
            {
                list.Add(new BEIdTexto
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Texto = Convert.ToString(r["Texto"])
                });
            }
            return list;
        }
        public List<BENombre> Buscar(string texto)
        {
            var h = new Hashtable { { "@Texto", (object)texto ?? DBNull.Value } };
            DataTable dt = _datos.Leer("sp_Usuario_Buscar", h);

            var list = new List<BENombre>();
            foreach (DataRow r in dt.Rows)
            {
                var u = new BENombre();
                u.Id = Convert.ToInt32(r["Id"]);
                u.Email = Convert.ToString(r["Email"]);
                u.EmailVerified = r["EmailVerified"] != DBNull.Value && Convert.ToBoolean(r["EmailVerified"]);
                u.Activo = r.Table.Columns.Contains("Activo") && r["Activo"] != DBNull.Value ? Convert.ToBoolean(r["Activo"]) : true;
                u.Nombre = r["Nombre"] == DBNull.Value ? null : Convert.ToString(r["Nombre"]);
                u.Apellido = r["Apellido"] == DBNull.Value ? null : Convert.ToString(r["Apellido"]);
                list.Add(u);
            }
            return list;
        }

        public bool SetActive(int userId, bool isActive)
        {
            var h = new Hashtable { { "@UserId", userId }, { "@IsActive", isActive } };
            int afectadas = _datos.LeerCantidad("sp_Usuario_SetActive", h);
            return afectadas > 0;
        }
    }
}

