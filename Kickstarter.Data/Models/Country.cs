using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Country
    {
        public Country()
        {
            Address = new HashSet<Address>();
            Zone = new HashSet<Zone>();
        }

        public int CountryId { get; set; }
        public string Name { get; set; }
        public string IsoCode2 { get; set; }
        public string IsoCode3 { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<Zone> Zone { get; set; }
    }
}
