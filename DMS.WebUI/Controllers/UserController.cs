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
                 CategoryLinks = catuserlinks.ToList(), 
                Permissions = GetPermissionString(user.CanEditDocument, user.CanRemoveDocument, user.CanEditCategory, user.CanRemoveCategory, user.CanEditUser, user.CanRemoveUser)
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
                SelectedCategories = user.CategoryLinks.Select(c => c.CategoryId).ToList(),
                CanEditDocument = GetPermission(user.Permissions, UserPermission.CanEditDocument),
                CanRemoveDocument = GetPermission(user.Permissions, UserPermission.CanRemoveDocument),
                CanEditCategory = GetPermission(user.Permissions, UserPermission.CanEditCategory),
                CanRemoveCategory = GetPermission(user.Permissions, UserPermission.CanRemoveCategory),
                CanEditUser = GetPermission(user.Permissions, UserPermission.CanEditUser),
                CanRemoveUser = GetPermission(user.Permissions, UserPermission.CanRemoveUser),
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
                CategoryLinks = catuserlinks.ToList(),
                Permissions = GetPermissionString(user.CanEditDocument, user.CanRemoveDocument, user.CanEditCategory, user.CanRemoveCategory, user.CanEditUser, user.CanRemoveUser)
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


        private bool GetPermission(string permissionString, UserPermission permission)
        {
            //To make it simple, we are keeping a fixed position of each permission in a string
            // canEditdoc, canRemoveDoc, canEditCat, canRemoveCat, canEditUser, canRemoveUser

            bool result = false;
            if(string.IsNullOrEmpty(permissionString) || permissionString.Length < 6)
            {
                return result;
            }
            switch (permission)
            {
                case UserPermission.CanEditDocument:
                    result = permissionString.ElementAt(0) == '1';
                    break;
                case UserPermission.CanRemoveDocument:
                    result = permissionString.ElementAt(1) == '1';
                    break;
                case UserPermission.CanEditCategory:
                    result = permissionString.ElementAt(2) == '1';
                    break;
                case UserPermission.CanRemoveCategory:
                    result = permissionString.ElementAt(3) == '1';
                    break;
                case UserPermission.CanEditUser:
                    result = permissionString.ElementAt(4) == '1';
                    break;
                case UserPermission.CanRemoveUser:
                    result = permissionString.ElementAt(5) == '1';
                    break;

            }
            return result;
        }

        private string GetPermissionString(bool canEditDoc, bool canRemoveDoc, bool canEditCat, bool canRemoveCat, bool canEditUser, bool canRemoveUser)
        {
            return Convert.ToInt32(canEditDoc).ToString() + Convert.ToInt32(canRemoveDoc).ToString()
                + Convert.ToInt32(canEditCat).ToString() + Convert.ToInt32(canRemoveCat).ToString()
                + Convert.ToInt32(canEditUser).ToString() + Convert.ToInt32(canRemoveUser).ToString();
        }

        private enum UserPermission
        {
            CanEditDocument, CanRemoveDocument, CanEditCategory, CanRemoveCategory, CanEditUser, CanRemoveUser
        }

    }
}