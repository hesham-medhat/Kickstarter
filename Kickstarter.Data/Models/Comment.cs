using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Comment
    {
        public uint Id { get; set; }
        public uint PostId { get; set; }
        public uint UserId { get; set; }

        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
