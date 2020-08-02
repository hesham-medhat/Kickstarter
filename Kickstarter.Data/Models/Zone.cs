using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Zone
    {
        public Zone()
        {
            Address = new HashSet<Address>();
        }

        public int ZoneId { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<Address> Address { get; set; }
    }
}
