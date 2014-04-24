using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using BookKeeping.Domain.Repositories;
using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Services.Security
{
    public class Permissions
    {
        public string UserId { get; set; }

        public bool IsUserSuperAdmin { get; private set; }

        public GeneralPermissionType GeneralPermissions { get; set; }

        public Dictionary<long, StoreSpecificPermissionType> StoreSpecificPermissions { get; set; }

        public Permissions(string userId, bool isUserSuperAdmin)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(userId), "userId");
            this.UserId = userId;
            this.IsUserSuperAdmin = isUserSuperAdmin;
            this.StoreSpecificPermissions = new Dictionary<long, StoreSpecificPermissionType>();
        }

        public bool HasPermission(GeneralPermissionType permissionType)
        {
            if (!this.IsUserSuperAdmin)
                return this.GeneralPermissions.HasFlag(permissionType);
            else
                return true;
        }

        public bool HasPermission(StoreSpecificPermissionType permissionType, long storeId)
        {
            if (this.IsUserSuperAdmin)
                return true;
            if (this.StoreSpecificPermissions != null)
                return Enumerable.Any<KeyValuePair<long, StoreSpecificPermissionType>>((IEnumerable<KeyValuePair<long, StoreSpecificPermissionType>>)this.StoreSpecificPermissions, (Func<KeyValuePair<long, StoreSpecificPermissionType>, bool>)(kvp =>
                {
                    if (kvp.Key == storeId)
                        return kvp.Value.HasFlag((Enum)permissionType);
                    else
                        return false;
                }));
            else
                return false;
        }

        public void Save()
        {
            if (this.IsUserSuperAdmin)
                throw new InvalidOperationException("Permissions for a super admin can't be saved");
            DependencyResolver.Current.GetService<IPermissionRepository>().Save(this);
        }
    }
}