using SOCApi.Models;
using SOCApi.Interfaces;

namespace SOCApi.Services
{
    public class PermissionService : IPermissionService
    {
        public void AddPermission(string userName, Permission permission)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Permission> GetUserPermissions(string userName)
        {
            throw new NotImplementedException();
        }

        public void RemovePermission(string userName, Permission permission)
        {
            throw new NotImplementedException();
        }
    }
}
