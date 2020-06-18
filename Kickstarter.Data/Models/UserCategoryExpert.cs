using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class UserCategoryExpert
    {
        public uint UserId { get; set; }
        public uint CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
    }
}
