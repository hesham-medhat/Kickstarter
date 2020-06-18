using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Category
    {
        public Category()
        {
            Post = new HashSet<Post>();
            UserCategoryExpert = new HashSet<UserCategoryExpert>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<UserCategoryExpert> UserCategoryExpert { get; set; }
    }
}
