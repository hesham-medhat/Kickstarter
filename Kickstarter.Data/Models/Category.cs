using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Category
    {
        public Category()
        {
            ExpertToCategory = new HashSet<ExpertToCategory>();
            Post = new HashSet<Post>();
        }

        public string Category1 { get; set; }

        public virtual ICollection<ExpertToCategory> ExpertToCategory { get; set; }
        public virtual ICollection<Post> Post { get; set; }
    }
}
