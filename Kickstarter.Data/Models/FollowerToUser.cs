using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class FollowerToUser
    {
        public int FollowerId { get; set; }
        public int UserId { get; set; }

        public virtual User Follower { get; set; }
        public virtual User User { get; set; }
    }
}
