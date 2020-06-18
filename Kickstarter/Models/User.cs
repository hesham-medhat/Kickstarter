using System;
using System.Collections.Generic;

namespace Kickstarter.Models
{
    public partial class User
    {
        public User()
        {
            Comment = new HashSet<Comment>();
            FollowerToUserFollower = new HashSet<FollowerToUser>();
            FollowerToUserUser = new HashSet<FollowerToUser>();
            Post = new HashSet<Post>();
            UserCategoryExpert = new HashSet<UserCategoryExpert>();
            UserTagExpert = new HashSet<UserTagExpert>();
            UserVotes = new HashSet<UserVotes>();
        }

        public uint Id { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public uint Points { get; set; }
        public DateTime CreatedAt { get; set; }
        public uint AddressId { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<FollowerToUser> FollowerToUserFollower { get; set; }
        public virtual ICollection<FollowerToUser> FollowerToUserUser { get; set; }
        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<UserCategoryExpert> UserCategoryExpert { get; set; }
        public virtual ICollection<UserTagExpert> UserTagExpert { get; set; }
        public virtual ICollection<UserVotes> UserVotes { get; set; }
    }
}
