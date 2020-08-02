using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class ExpertToCategory
    {
        public string Username { get; set; }
        public string Category { get; set; }

        public virtual Category CategoryNavigation { get; set; }
        public virtual User UsernameNavigation { get; set; }
    }
}
