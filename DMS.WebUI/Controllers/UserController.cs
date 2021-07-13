using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DMS.Data;
using DMS.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using DMS.WebUI.Models;

namespace DMS.Controllers
{
    /*
     * USER CONTROLLER MAIN CLASS
     * HAS ACCESS TO:ADMIN
     */
    [Authorize(Policy= "Admin")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private CategoryService _categoryService;
        public UserController(DMSContext _context, IConfiguration _config)
        {
            _userService = new UserService(_context, _config);
            _categoryService = new CategoryService(_context);
        }
       
        /*
         * GET LIST OF USERS
         */
        public IActionResult Index()
        {
           var users=  _userService.GetAll();
            return View(users);
        }

        /*
         * NEW USER CREATE FORM
         */
        public IActionResult Create()
        {
            var categories = _categoryService.GetAll();
            ViewBag.Categories = new MultiSelectList(categories, "CategoryId", "CategoryName");            
            return View();
        }

        /*
         * CREATE NEW USER
         */
        [HttpPost]
        public IActionResult Create(UserModel user)
        {
            var categories = _categoryService.GetAll();
            var catuserlinks = user.SelectedCategories.Select(c =>
            {
                return new CategoryUser() { CategoryId = c, UserId = user.UserId };
            });
            var status = _userService.Create(new Data.User() {
                 UserEmail = user.UserEmail, UserName = user.UserName, password = user.password, UserRole = user.UserRole, 
                 CategoryLinks = catuserlinks.ToList()
            });
            if (status)
            {
                ViewBag.success = "Created successfully";
            }
            else
            {
                ViewBag.error = "Error Occurred";
            }
            ViewBag.Categories = new MultiSelectList(categories, "CategoryId", "CategoryName");
            return View();
        }

        /*
         * DELETE USER BY ID
         */
        public IActionResult Delete(int id)
        {
            var status = _userService.Delete(id);
            if (status)
            {
                ViewBag.success = "Deleted successfully";
            }
            else
            {
                ViewBag.error = "Error Occurred";
            }
            return RedirectToAction("Index");
        }


        /*
         * EDIT USER FORM
         */
        public IActionResult Edit(int id)
        {
            var categories = _categoryService.GetAll();
            var user = _userService.Get(id);

            ViewBag.Categories = new MultiSelectList(categories, "CategoryId", "CategoryName");

            var userModel = new UserModel()
            {
                UserEmail = user.UserEmail,
                UserName = user.UserName,
                password = user.password,
                UserRole = user.UserRole,
                UserId = user.UserId,
                SelectedCategories = user.CategoryLinks.Select(c => c.CategoryId).ToList()
        };

            return View(userModel);
        }

        /*
         * EDIT USER
         */
        [HttpPost]
        public IActionResult Edit(UserModel user)
        {
            var categories = _categoryService.GetAll();
            var catuserlinks = user.SelectedCategories.Select(c =>
            {
                return new CategoryUser() { CategoryId = c, UserId = user.UserId };
            });
            var status = _userService.Update(new Data.User()
            {
                UserId = user.UserId,
                UserEmail = user.UserEmail,
                UserName = user.UserName,
                password = user.password,
                UserRole = user.UserRole,
                CategoryLinks = catuserlinks.ToList()
            });
            if (status)
            {
                ViewBag.success = "Updated successfully";
            }
            else
            {
                ViewBag.error = "Error Occurred";
            }
            ViewBag.Categories = new MultiSelectList(categories, "CategoryId", "CategoryName");
            return View(user);
        }


    }
}