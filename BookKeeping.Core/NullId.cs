using System.Runtime.Serialization;

namespace BookKeeping.Core
{
    [DataContract(Namespace = "BookKeeping")]
    public sealed class NullId : IIdentity
    {
        public const string TagValue = "";
        public static readonly IIdentity Instance = new NullId();

        public string GetId()
        {
            return "";
        }

        public string GetTag()
        {
            return "";
        }

        public int GetStableHashCode()
        {
            return 42;
        }
    }
}