using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class UserTagExpert
    {
        public uint UserId { get; set; }
        public uint TagId { get; set; }

        public virtual Tag Tag { get; set; }
        public virtual User User { get; set; }
    }
}
