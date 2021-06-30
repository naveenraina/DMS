using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Data;
using System.Security.Claims;

namespace DMS.Service
{
    /*
     * CATEGORY SERVICE MAIN CLASS
     */
    public class CategoryService
    {
        private readonly DMSContext _context;
        public CategoryService(DMSContext db)
        {
           _context = db;
        }

        /*
         * GET LIST OF CATEGORY
         */
        public List<Category> GetAll()
        {
            var cats = _context.Categories.ToList();
            return cats;
        }

        /*
         * GET LIST OF CATEGORY for email
         */
        public List<Category> GetAll(string email)
        {
            var user = _context.Users.Include(u => u.CategoryLinks).Where(x => x.UserEmail == email).FirstOrDefault();
            IQueryable<Category> usercats;
            if(user.UserRole == "Admin")
            {
                usercats = from c in _context.Categories
                               select c;                
            } 
            else
            {
                usercats = from c in _context.Categories
                               join cl in user.CategoryLinks on c.CategoryId equals cl.CategoryId
                               where cl.UserId == user.UserId
                               select c;
            }
            var cats = _context.Categories.Where(x => usercats.Contains(x)).ToList();
            return cats;
        }

        /*
         * CREATE CATEGORY
         */
        public bool CreateCategory(Category Cat, string email)
        {
            bool status;
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
            Category item = new Category();
            item.CategoryName = Cat.CategoryName;
                      
            try
            {
                _context.Categories.Add(item);
                _context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                var exp = ex;
                status = false;
            }
            return status;
        }

        /*
         * DELETE CATEGORY
         */
        public bool DeleteCategory(int id)
        {
            bool status;
            var item = _context.Categories.Find(id);
            
            try
            {
                _context.Categories.Remove(item);
                _context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                var exp = ex;
                status = false;
            }
            return status;
        }

    }
}
