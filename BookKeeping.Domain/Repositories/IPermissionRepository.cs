using BookKeeping.Services.Security;

namespace BookKeeping.Domain.Repositories
{
    public interface IPermissionRepository
    {
        Permissions Get(string userId);

        void Save(Permissions permission);
    }
}