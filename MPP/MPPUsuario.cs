// MPP/MPPUsuario.cs
using System;
using System.Collections;
using System.Data;
using BE;
using DAL;

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

        /// <summary>Trae usuario por email, incluyendo PasswordHash para validar login.</summary>
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
                PasswordHash = Convert.ToString(r["PasswordHash"])
            };

            // Cargar roles
            u.Roles = GetRoles(u.Id);
            return u;
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
        public int CrearUsuario(string email, string passwordHash, bool emailVerified)
        {
            var h = new Hashtable
            {
                { "@Email", email },
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
    }
}
