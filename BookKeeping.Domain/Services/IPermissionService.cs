namespace BookKeeping.Services.Security
{
    public interface IPermissionService
    {
        Permissions GetCurrentLoggedInUserPermissions();

        Permissions Get(string userId);
    }
}