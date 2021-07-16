using System;
using System.Collections.Generic;

namespace SampleAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public int? CompanyId { get; set; }

        public Company? Company { get; set; }

        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public virtual ICollection<Post> Posts { get; } = new List<Post>();

        public DateTime CreateDate { get; set; }
    }
}
