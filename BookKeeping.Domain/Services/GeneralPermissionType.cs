using System;

namespace BookKeeping.Services.Security
{
    [Flags]
    public enum GeneralPermissionType
    {
        None = 0,
        AccessSecurity = 1,
        AccessLicenses = 2,
        CreateAndDeleteStore = 4,
    }
}