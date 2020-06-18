using System;
using System.Collections.Generic;

namespace Kickstarter.Models
{
    public partial class Country
    {
        public Country()
        {
            Zone = new HashSet<Zone>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Zone> Zone { get; set; }
    }
}
