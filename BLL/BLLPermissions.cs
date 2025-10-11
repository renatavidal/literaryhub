// BLL/BLLPermissions.cs
using System;
using System.Data;
using MPP;

namespace BLL
{
    public class BLLPermissions
    {
        private readonly MPPPermissions _mpp = new MPPPermissions();
        public DataTable Roles() => _mpp.Roles();
        public DataTable Permissions() => _mpp.Permissions();
        public DataTable RolePerms(int roleId) => _mpp.RolePerms(roleId);
        public void Grant(int roleId, int permId) => _mpp.Grant(roleId, permId);
        public void Revoke(int roleId, int permId) => _mpp.Revoke(roleId, permId);

        public DataTable AdminUsers() => _mpp.AdminUsers();
        public void AdminGrantRole(int userId, int roleId) => _mpp.AdminGrantRole(userId, roleId);
        public void AdminRevokeRole(int userId, int roleId) => _mpp.AdminRevokeRole(userId, roleId);
    }
}
