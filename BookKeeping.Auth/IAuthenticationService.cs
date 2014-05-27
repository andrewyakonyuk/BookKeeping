using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.Auth
{
    public interface IAuthenticationService
    {
        bool SignIn(string username, string password);
        void SignOut();
    }
}
