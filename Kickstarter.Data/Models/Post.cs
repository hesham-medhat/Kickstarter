using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Post
    {
        public Post()
        {
            Comment = new HashSet<Comment>();
            PostToTag = new HashSet<PostToTag>();
            UserVotes = new HashSet<UserVotes>();
        }

        public uint Id { get; set; }
        public uint UserId { get; set; }
        public uint CategoryId { get; set; }
        public string State { get; set; }

        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<PostToTag> PostToTag { get; set; }
        public virtual ICollection<UserVotes> UserVotes { get; set; }
    }
}
