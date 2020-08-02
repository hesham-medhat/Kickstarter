﻿using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Post
    {
        public string PostId { get; set; }
        public string Username { get; set; }
        public string Category { get; set; }

        public virtual Category CategoryNavigation { get; set; }
        public virtual User UsernameNavigation { get; set; }
    }
}
