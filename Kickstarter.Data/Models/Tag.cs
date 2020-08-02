using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Tag
    {
        public string Name { get; set; }
        public int Occurrences { get; set; }
    }
}
