using System;
using System.Collections.Generic;

namespace SampleAPI.Models
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
