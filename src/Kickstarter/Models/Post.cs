using System;
using System.Collections.Generic;

namespace Kickstarter.Models
{
    public partial class Post
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public uint CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
    }
}
