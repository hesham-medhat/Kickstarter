using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Comment
    {
        public string CommentId { get; set; }
        public string PostId { get; set; }
        public int UserId { get; set; }
        public DateTime DateAdded { get; set; }
        public string Description { get; set; }

        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
