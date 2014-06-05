using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    public class Role
    {
        public enum ROLES : int {
            ANONYMOUS_USER_ROLE = 0,
            REGISTERED_USER_ROLE = 1,
            BETTING_USER_ROLE = 2,
            ADMINISTRATOR_USER_ROLE = 3,
            CONNECTION_USER_ROLE = 99
        };
    }
}
