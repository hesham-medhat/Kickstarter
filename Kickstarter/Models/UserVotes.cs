using System;
using System.Collections.Generic;

namespace Kickstarter.Models
{
    public partial class UserVotes
    {
        public uint UserId { get; set; }
        public uint PostId { get; set; }
        public ulong Direction { get; set; }

        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
