﻿using DMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.WebUI.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string password { get; set; }
        public string UserRole { get; set; }
        public List<Category> Categories { get; set; }
        public List<int> SelectedCategories { get; set; }
        public int[] SelectedCategoriesArray { get; set; }

        public bool CanEditDocument { get; set; }
        public bool CanRemoveDocument { get; set; }
        public bool CanEditCategory{ get; set; }
        public bool CanRemoveCategory { get; set; }
        public bool CanEditUser{ get; set; }
        public bool CanRemoveUser { get; set; }
    }
}
