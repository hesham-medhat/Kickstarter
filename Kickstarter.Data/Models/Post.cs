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

        public string PostId { get; set; }
        public string Username { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }

        public virtual Category CategoryNavigation { get; set; }
        public virtual User UsernameNavigation { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<PostToTag> PostToTag { get; set; }
        public virtual ICollection<UserVotes> UserVotes { get; set; }
    }
}
