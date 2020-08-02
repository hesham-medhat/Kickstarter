using System;
using System.Collections.Generic;

namespace Kickstarter.Data.Models
{
    public partial class Address
    {
        public Address()
        {
            User = new HashSet<User>();
        }

        public int AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public int CountryId { get; set; }
        public int ZoneId { get; set; }

        public virtual Country Country { get; set; }
        public virtual Zone Zone { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
