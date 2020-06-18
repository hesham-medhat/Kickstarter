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

        public uint Id { get; set; }
        public uint CountryId { get; set; }
        public string Name { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<Address> Address { get; set; }
    }
}
