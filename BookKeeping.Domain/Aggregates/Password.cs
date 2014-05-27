﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BookKeeping.Domain.Aggregates
{
    public sealed class Password
    {
        private static readonly Random random = new Random();
        private static readonly HashAlgorithm hashAlgorithm = MD5.Create();

        [Obsolete("Only for NHibernate")]
        protected Password()
        {
        }

        public Password(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }

        public Password(string password)
        {
            Salt = MakeSalt();
            Hash = HashPassword(password, Salt);
        }

        public string Hash { get; private set; }

        public string Salt { get; private set; }

        public bool Check(string oldPassword)
        {
            return Hash == HashPassword(oldPassword, Salt);
        }

        private static string HashPassword(string password, string salt)
        {
            Encoding encoding = Encoding.UTF8;
            byte[] passwordBytes = encoding.GetBytes(password)
                .Union(salt.FromBase64())
                .ToArray();
            return hashAlgorithm.ComputeHash(passwordBytes).ToBase64();
        }

        private static string MakeSalt()
        {
            return Enumerable.Range(0, 5)
                .Select(_ => (byte)random.Next())
                .ToBase64();
        }
    }
}
