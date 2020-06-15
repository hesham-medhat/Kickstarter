using System;
using System.Collections.Generic;

namespace Kickstarter.Models
{
    public partial class Category
    {
        public Category()
        {
            Post = new HashSet<Post>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Post> Post { get; set; }
    }
}
