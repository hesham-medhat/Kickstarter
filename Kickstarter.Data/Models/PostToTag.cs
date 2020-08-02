using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class PostToTag
    {
        public string PostId { get; set; }
        public string Tag { get; set; }
    }
}
