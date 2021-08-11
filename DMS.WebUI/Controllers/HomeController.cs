using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DMS.Models;
using DMS.Data;
using DMS.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DMS.Controllers
{
    /*
     * HOME CONTROLLER MAIN CLASS
     */
    [Authorize]
    public class HomeController : Controller
    {

        private readonly DocumentService _documentService;
        private CategoryService _categoryService;
        private UserService _userService;
        public HomeController(DMSContext context, IConfiguration _config)
        {
            _documentService = new DocumentService(context);
            _categoryService = new CategoryService(context);
            _userService = new UserService(context, _config);
        }

        /*
         * HOMEPAGE
         */
        public IActionResult Index()
        {
            ViewBag.Stats = _documentService.GetStats();
            ViewBag.CatCount = _categoryService.TotalCount();
            ViewBag.UserCount = _userService.TotalCount();
            return View();
        }

        /*
         * ERROR
         */
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
