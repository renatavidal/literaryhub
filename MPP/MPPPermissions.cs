// MPP/MPPPermissions.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DAL;

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
    }
}
