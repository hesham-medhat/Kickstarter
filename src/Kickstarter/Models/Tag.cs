using System;
using System.Collections.Generic;

namespace Kickstarter.Models
{
    public partial class Tag
    {
        public uint Id { get; set; }
        public string NormalizedName { get; set; }
    }
}
