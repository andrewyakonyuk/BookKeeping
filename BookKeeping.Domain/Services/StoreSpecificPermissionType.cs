using System;

namespace BookKeeping.Services.Security
{
    [Flags]
    public enum StoreSpecificPermissionType
    {
        None = 0,
        AccessStore = 1,
        AccessSettings = 2,
    }
}