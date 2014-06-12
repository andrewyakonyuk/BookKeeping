using BookKeeping.Domain.Aggregates;
using BookKeeping.Domain.Contracts;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace BookKeeping.Auth
{
    public class AccountEntry
    {
        [Obsolete("Only for serializations")]
        public AccountEntry()
        {
        }

        public AccountEntry(User account)
        {
            Name = account.Login;
            Id = account.Id;
            RoleType = account.Role;
        }

        public string Name { get; set; }

        public UserId Id { get; set; }

        public string RoleType { get; set; }

        public string Serialize()
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new XmlSerializer(typeof(AccountEntry));
                formatter.Serialize(stream, this);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static AccountEntry Deserialize(string value)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
            {
                var formatter = new XmlSerializer(typeof(AccountEntry));

                return (AccountEntry)formatter.Deserialize(stream);
            }
        }
    }
}
