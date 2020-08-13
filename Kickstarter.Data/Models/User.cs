using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class User
    {
        public User()
        {
            Comment = new HashSet<Comment>();
            ExpertToCategory = new HashSet<ExpertToCategory>();
            FollowerToUserFollower = new HashSet<FollowerToUser>();
            FollowerToUserUser = new HashSet<FollowerToUser>();
            Post = new HashSet<Post>();
            UserVotes = new HashSet<UserVotes>();
        }

        public int UserId { get; set; }
        public int UserGroupId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public int AddressId { get; set; }
        public string Telephone { get; set; }
        public int Points { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual Address Address { get; set; }
        public virtual UserGroup UserGroup { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<ExpertToCategory> ExpertToCategory { get; set; }
        public virtual ICollection<FollowerToUser> FollowerToUserFollower { get; set; }
        public virtual ICollection<FollowerToUser> FollowerToUserUser { get; set; }
        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<UserVotes> UserVotes { get; set; }
    }
}
