using System;
using System.Collections.Generic;

namespace Kickstarter.Models
{
    public partial class User
    {
        public User()
        {
            Post = new HashSet<Post>();
        }

        public uint Id { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public uint Points { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Post> Post { get; set; }
    }
}
