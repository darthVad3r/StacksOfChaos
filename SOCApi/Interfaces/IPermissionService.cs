using SOCApi.Models;

namespace SOCApi.Interfaces
{
    public interface IPermissionService
    {
        public void AddPermission(string userName, Permission permission);
        public void RemovePermission(string userName, Permission permission);
        public IEnumerable<Permission> GetUserPermissions(string userName);
    }
}
