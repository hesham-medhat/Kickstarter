using System;
using System.Collections.Generic;

namespace Kickstarter.Models
{
    public partial class FollowerToUser
    {
        public uint FollowerId { get; set; }
        public uint UserId { get; set; }

        public virtual User Follower { get; set; }
        public virtual User User { get; set; }
    }
}
