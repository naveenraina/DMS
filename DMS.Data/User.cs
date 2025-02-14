﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Data
{
    public partial class User
    {
        public User()
        {
            this.CategoryLinks = new HashSet<CategoryUser>();
            this.Documents = new HashSet<Document>();
        }
        public static ClaimsIdentity Identity { get; set; }
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string UserRole { get; set; }
        public string Permissions { get; set; }
        public virtual ICollection<CategoryUser> CategoryLinks { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}
