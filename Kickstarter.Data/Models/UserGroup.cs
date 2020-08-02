using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class UserGroup
    {
        public UserGroup()
        {
            User = new HashSet<User>();
        }

        public int UserGroupId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
