namespace BookKeeping.Domain.Contracts
{
    public interface IUserIdentity : System.Security.Principal.IIdentity
    {
        UserId Id { get; }

        string RoleType { get; }

        string[] GetRoles();
    }
}