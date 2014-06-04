using BookKeeping.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BookKeeping.Projections.UserList
{
    [DataContract]
    [Serializable]
    public class UserListView
    {
        [DataMember(Order = 1)]
        public List<UserView> Users { get; private set; }

        public UserListView()
        {
            Users = new List<UserView>();
        }
    }

    [DataContract]
    [Serializable]
    public class UserView
    {
        [DataMember(Order = 1)]
        public UserId Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Login { get; set; }

        [DataMember(Order = 4)]
        public string RoleType { get; set; }
    }
}
