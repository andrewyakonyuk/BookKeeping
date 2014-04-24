using BookKeeping.Infrastructure.Dependency;

namespace BookKeeping.Services.Security
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionProvider _permissionProvider;

        public static IPermissionService Instance
        {
            get
            {
                return DependencyResolver.Current.GetService<IPermissionService>();
            }
        }

        public PermissionService(IPermissionProvider permissionProvider)
        {
            this._permissionProvider = permissionProvider;
        }

        public Permissions GetCurrentLoggedInUserPermissions()
        {
            return this._permissionProvider.GetCurrentLoggedInUserPermissions();
        }

        public Permissions Get(string userId)
        {
            return this._permissionProvider.Get(userId);
        }
    }
}