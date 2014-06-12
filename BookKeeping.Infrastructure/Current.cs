using System;
using System.Diagnostics.Contracts;
using System.Threading;
using BookKeeping.Domain.Contracts;

namespace BookKeeping.Infrastructure
{
    public static class Current
    {
        static Func<DateTime> _getTime = GetUtc;
        static Func<Guid> _getGuid = GetGuid;
        static Func<IUserIdentity> _getIdentity = GetIdentity;

        static DateTime GetUtc()
        {
            return new DateTime(DateTime.UtcNow.Ticks, DateTimeKind.Unspecified);
        }

        static Guid GetGuid()
        {
            return Guid.NewGuid();
        }

        static IUserIdentity GetIdentity()
        {
            if (Thread.CurrentPrincipal == null)
                return null;
            return Thread.CurrentPrincipal.Identity as IUserIdentity;
        }

        public static void DateIs(DateTime time)
        {
            _getTime = () => new DateTime(time.Ticks, DateTimeKind.Unspecified);
        }

        public static readonly DateTime MaxValue = new DateTime(9999, 12, 31, 0, 0, 0, DateTimeKind.Unspecified);

        public static void DateIs(int year, int month = 1, int day = 1)
        {
            Contract.Requires<ArgumentException>(year >= 1 && year <= 9999);
            Contract.Requires<ArgumentException>(month >= 1 && month <= 12);
            Contract.Requires<ArgumentException>(day >= 1 && day <= 31);
            DateIs(new DateTime(year, month, day));
        }

        public static void GuidIs(Guid value)
        {
            _getGuid = () => value;
        }

        public static void GuidIs(string guid)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(guid));
            var g = Guid.Parse(guid);
            _getGuid = () => g;
        }

        public static void IdentityIs(IUserIdentity identity)
        {
            _getIdentity = () => identity;
        }

        public static void Reset()
        {
            _getTime = GetUtc;
            _getGuid = GetGuid;
            _getIdentity = GetIdentity;
        }

        public static IUserIdentity Identity { get { return _getIdentity(); } }
        public static DateTime UtcNow { get { return _getTime(); } }
        public static Guid NewGuid { get { return _getGuid(); } }
    }
}
