using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class UserVotes
    {
        public string Username { get; set; }
        public string PostId { get; set; }
        public string Direction { get; set; }

        public virtual Post Post { get; set; }
        public virtual User UsernameNavigation { get; set; }
    }
}
