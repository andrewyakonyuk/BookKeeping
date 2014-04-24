namespace BookKeeping.Services.Security
{
    public interface IPermissionProvider
    {
        Permissions GetCurrentLoggedInUserPermissions();

        Permissions Get(string userId);
    }
}