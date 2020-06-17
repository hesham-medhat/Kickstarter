using System;
using System.Collections.Generic;

namespace Kickstarter.Models
{
    public partial class PostToTag
    {
        public uint PostId { get; set; }
        public uint TagId { get; set; }

        public virtual Post Post { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
