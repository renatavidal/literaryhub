// MPP/MPPPermissions.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DAL;
using BE;

namespace MPP
{
    public class MPPPermissions
    {
        private readonly Acceso _db = new Acceso();

        public DataTable Roles() => _db.Leer("usp_Roles_List", new Hashtable());
        public DataTable Permissions() => _db.Leer("usp_Permissions_List", new Hashtable());
        public DataTable RolePerms(int roleId)
          => _db.Leer("usp_RolePermissions_List", new Hashtable { { "@RoleId", roleId } });

        public void Grant(int roleId, int permId)
          => _db.Escribir("usp_RolePermission_Grant", new Hashtable { { "@RoleId", roleId }, { "@PermId", permId } });
        public void Revoke(int roleId, int permId)
          => _db.Escribir("usp_RolePermission_Revoke", new Hashtable { { "@RoleId", roleId }, { "@PermId", permId } });

        public DataTable AdminUsers() => _db.Leer("usp_AdminUsers_List", new Hashtable());
        public void AdminGrantRole(int userId, int roleId)
          => _db.Escribir("usp_AdminUser_GrantRole", new Hashtable { { "@UserId", userId }, { "@RoleId", roleId } });
        public void AdminRevokeRole(int userId, int roleId)
          => _db.Escribir("usp_AdminUser_RevokeRole", new Hashtable { { "@UserId", userId }, { "@RoleId", roleId } });
        public int Role_Crear(string nombre)
        {
            var h = new Hashtable { { "@Nombre", nombre } };
            return _db.LeerCantidad("sp_Role_Insert", h);
        }
        public System.Data.DataTable Users_Listar()
        {
            var h = new System.Collections.Hashtable { { "@Texto", DBNull.Value } };
            return _db.Leer("s_usuarios_buscar", h); // NULL => TODOS
        }

        public System.Data.DataTable Usuarios_Buscar(string texto)
        {
            var h = new System.Collections.Hashtable { { "@Texto", (object)texto ?? DBNull.Value } };
            return _db.Leer("s_usuarios_buscar", h);
        }
    }
}
