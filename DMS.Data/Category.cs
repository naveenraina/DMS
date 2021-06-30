using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DMS.Data
{
   public partial class Category
    {
        public Category()
        {
            this.UserLinks = new HashSet<CategoryUser>();
        }

        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<CategoryUser> UserLinks { get; set; }
    }
}
