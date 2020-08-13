using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Tag
    {
        public Tag()
        {
            PostToTag = new HashSet<PostToTag>();
        }

        public string Tag1 { get; set; }
        public int Occurrences { get; set; }

        public virtual ICollection<PostToTag> PostToTag { get; set; }
    }
}
