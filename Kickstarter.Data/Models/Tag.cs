using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Tag
    {
        public Tag()
        {
            PostToTag = new HashSet<PostToTag>();
            UserTagExpert = new HashSet<UserTagExpert>();
        }

        public uint Id { get; set; }
        public string NormalizedName { get; set; }

        public virtual ICollection<PostToTag> PostToTag { get; set; }
        public virtual ICollection<UserTagExpert> UserTagExpert { get; set; }
    }
}
