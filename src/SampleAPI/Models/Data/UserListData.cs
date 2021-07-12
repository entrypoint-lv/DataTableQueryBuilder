using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Models.Data
{
    public class UserListData
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int Posts { get; set; }
        public DateTime CreateDate { get; set; }
    }
}